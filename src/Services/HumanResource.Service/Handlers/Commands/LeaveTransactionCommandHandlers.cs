using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class LeaveTransactionV3CommandHandlers :
        IRequestHandler<CreateLeaveTransactionV3Command, LeaveTransactionV3Dto>,
        IRequestHandler<UpdateLeaveTransactionV3Command, bool>,
        IRequestHandler<DeleteLeaveTransactionV3Command, bool>
    {
        private const int ApprovedStatusId = 582;
        // Special approval is required for a leave of three days or more.
        // Keep this aligned with the Leave Transaction UI.
        private const double CeoApprovalThresholdDays = 3;
        private const int InLieuValidityDays = 30;
        private const string CeoApprovalRequiredMessage = "30 days have passed since the in-lieu date. CEO approval is required for this leave.";
        private const string CompleteApprovalRequiresCeoMessage = "CEO approval is required to complete this leave approval.";

        private readonly HumanResourceDbContext _context;

        public LeaveTransactionV3CommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveTransactionV3Dto> Handle(CreateLeaveTransactionV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var lastId = await _context.LeaveTransactions.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0;
            var leaveRequest = request.RequestId.HasValue
                ? await _context.LeaveRequests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.RequestId.Value, cancellationToken)
                : null;
            var leaveDays = ResolveLeaveDays(request, leaveRequest);
            var effectiveLeaveTypeId = request.LeaveTypeId ?? leaveRequest?.LeaveTypeId;
            ValidateCeoApprovalForLongLeave(request, leaveDays);
            var resolvedAdmApproval = await ResolveAdmApprovalAsync(request, leaveDays, cancellationToken);

            await ValidateInLieuLeaveEligibilityAsync(
                request.StatusId,
                effectiveLeaveTypeId,
                leaveRequest?.InLieuDate,
                ResolveInLieuRequestDate(request, leaveRequest),
                cancellationToken);

            var transNo = request.TransactionNo;
            if (string.IsNullOrEmpty(transNo))
            {
                transNo = $"TRX-{(lastId + 1).ToString().PadLeft(7, '0')}";
            }

            var entity = new ETLeavetranLegacy
            {
                Id = lastId + 1,
                TransNo = transNo,
                TransDate = request.TransactionDate ?? DateTime.Now,
                RequestId = request.RequestId,
                EmployeeId = request.EmployeeId,
                LeaveUnitId = request.LeaveUnitId,
                LeaveTypeId = request.LeaveTypeId,
                FromDate = request.FromDate ?? leaveRequest?.FromDate,
                TillDate = request.TillDate ?? leaveRequest?.TillDate,
                LeaveDays = leaveDays,
                AdmApproval = resolvedAdmApproval,
                IsPaid = request.PaidLeave,
                StatusId = request.StatusId,
                EntityId = command.EntityId,
                ClientId = command.ClientId,
                UtBlock = false,
                UtSysGen = false
            };

            await _context.LeaveTransactions.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(entity);
        }

        public async Task<bool> Handle(UpdateLeaveTransactionV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var entity = await _context.LeaveTransactions.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (entity == null) return false;

            var leaveRequest = request.RequestId.HasValue
                ? await _context.LeaveRequests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.RequestId.Value, cancellationToken)
                : null;
            var leaveDays = ResolveLeaveDays(request, leaveRequest, entity.LeaveDays);
            var effectiveLeaveTypeId = request.LeaveTypeId ?? leaveRequest?.LeaveTypeId ?? entity.LeaveTypeId;
            ValidateCeoApprovalForLongLeave(request, leaveDays);
            var resolvedAdmApproval = await ResolveAdmApprovalAsync(request, leaveDays, cancellationToken);

            await ValidateInLieuLeaveEligibilityAsync(
                request.StatusId,
                effectiveLeaveTypeId,
                leaveRequest?.InLieuDate,
                ResolveInLieuRequestDate(request, leaveRequest),
                cancellationToken);

            entity.TransNo = string.IsNullOrEmpty(request.TransactionNo) ? entity.TransNo : request.TransactionNo;
            entity.TransDate = request.TransactionDate ?? entity.TransDate;
            entity.RequestId = request.RequestId;
            entity.EmployeeId = request.EmployeeId;
            entity.LeaveUnitId = request.LeaveUnitId;
            entity.LeaveTypeId = request.LeaveTypeId;
            entity.FromDate = request.FromDate;
            entity.TillDate = request.TillDate;
            entity.LeaveDays = leaveDays;
            entity.IsPaid = request.PaidLeave;
            entity.AdmApproval = resolvedAdmApproval;
            entity.StatusId = request.StatusId;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteLeaveTransactionV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.LeaveTransactions.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (entity == null) return false;

            _context.LeaveTransactions.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task ValidateInLieuLeaveEligibilityAsync(
            int? statusId,
            int? leaveTypeId,
            DateTime? inLieuDate,
            DateTime? requestDate,
            CancellationToken cancellationToken)
        {
            if (!statusId.HasValue || !leaveTypeId.HasValue || !inLieuDate.HasValue || !requestDate.HasValue)
            {
                return;
            }

            var status = await _context.Lovs
                .AsNoTracking()
                .Where(x => x.Id == statusId.Value)
                .Select(x => new { x.Code, x.Stxt, x.Ltxt })
                .FirstOrDefaultAsync(cancellationToken);
            var statusText = $"{status?.Code} {status?.Stxt} {status?.Ltxt}".ToLowerInvariant();
            var isAcceptedStatus =
                statusText.Contains("approved") ||
                statusText.Contains("accepted") ||
                statusText.Contains("accept");

            if (!isAcceptedStatus)
            {
                return;
            }

            var isHalfAdjustLeave = await _context.LeaveTypes
                .AsNoTracking()
                .Where(x => x.Id == leaveTypeId.Value)
                .Select(x => x.halfadjust)
                .FirstOrDefaultAsync(cancellationToken);

            if (isHalfAdjustLeave != true)
            {
                return;
            }

            var daysSinceInLieu = (requestDate.Value.Date - inLieuDate.Value.Date).TotalDays;
            if (daysSinceInLieu >= InLieuValidityDays)
            {
                throw new InvalidOperationException(CeoApprovalRequiredMessage);
            }
        }

        private static DateTime? ResolveInLieuRequestDate(
            CreateLeaveTransactionV3Dto request,
            ETLeaveRequestLegacy? leaveRequest)
        {
            return leaveRequest?.RequestDate
                ?? request.TransactionDate
                ?? request.FromDate
                ?? leaveRequest?.FromDate;
        }

        private static double ResolveLeaveDays(
            CreateLeaveTransactionV3Dto request,
            ETLeaveRequestLegacy? leaveRequest,
            double? currentLeaveDays = null)
        {
            return request.LeaveDays ?? leaveRequest?.LeaveDays ?? currentLeaveDays ?? 0;
        }

        private static bool RequiresCeoApproval(double? leaveDays)
        {
            return (leaveDays ?? 0) >= CeoApprovalThresholdDays;
        }

        private static bool IsLineManagerApproved(int? statusId)
        {
            return statusId == ApprovedStatusId;
        }

        private static bool HasApprovalValue(int? admApproval) => admApproval.HasValue && admApproval.Value != 0;

        // ETLeavetrans.admApproval is a foreign key to EREmployeeinfohdr.Id.  A
        // workflow/LOV status is therefore not evidence of CEO approval; only
        // the ID of a real approving employee is valid here.
        private static bool IsCeoApproved(int? statusId, int? admApproval) => HasApprovalValue(admApproval);

        private static bool IsFullyApproved(double? leaveDays, int? statusId, int? admApproval)
        {
            return RequiresCeoApproval(leaveDays)
                ? IsLineManagerApproved(statusId) && IsCeoApproved(statusId, admApproval)
                : IsLineManagerApproved(statusId);
        }

        private static void ValidateCeoApprovalForLongLeave(CreateLeaveTransactionV3Dto request, double? leaveDays)
        {
            if (RequiresCeoApproval(leaveDays) &&
                IsLineManagerApproved(request.StatusId) &&
                !IsCeoApproved(request.StatusId, request.AdmApproval))
            {
                throw new InvalidOperationException(CompleteApprovalRequiresCeoMessage);
            }
        }

        private async Task<int?> ResolveAdmApprovalAsync(CreateLeaveTransactionV3Dto request, double? leaveDays, CancellationToken cancellationToken)
        {
            if (!RequiresCeoApproval(leaveDays))
            {
                return null;
            }

            if (!HasApprovalValue(request.AdmApproval))
                return null;

            var approvingEmployeeExists = await _context.EmployeeProfiles
                .AsNoTracking()
                .AnyAsync(employee => employee.Id == request.AdmApproval!.Value && employee.UtBlock != true, cancellationToken);

            if (!approvingEmployeeExists)
            {
                throw new InvalidOperationException(
                    "CEO approval must contain the ID of an active employee; approval-status IDs cannot be saved as an approver.");
            }

            return request.AdmApproval;
        }

        private static LeaveTransactionV3Dto MapToDto(ETLeavetranLegacy entity) => new(
            entity.Id,
            entity.TransNo,
            entity.TransDate,
            entity.RequestId,
            null,
            entity.EmployeeId,
            null,
            null,
            null,
            null,
            null,
            entity.LeaveUnitId,
            null,
            entity.LeaveTypeId,
            null,
            0,
            0,
            0,
            entity.FromDate,
            entity.TillDate,
            entity.LeaveDays,
            entity.IsPaid,
            entity.StatusId,
            null,
            entity.AdmApproval,
            RequiresCeoApproval(entity.LeaveDays),
            IsLineManagerApproved(entity.StatusId),
            IsCeoApproved(entity.StatusId, entity.AdmApproval),
            IsFullyApproved(entity.LeaveDays, entity.StatusId, entity.AdmApproval),
            null,
            null,
            null,
            null
        );
    }
}

