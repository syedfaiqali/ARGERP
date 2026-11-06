using HumanResource.Service.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace HumanResource.Service.Queries
{
    public record GetPayrollPeriodsV3Query(int ClientId, int? EntityId) : IRequest<PaginationResponse<PayrollPeriodV3Dto>>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 100;
        public string? SearchTerm { get; init; }
    }
    public record GetPayrollPeriodByIdV3Query(int Id) : IRequest<PayrollPeriodHeaderV3Dto?>;
    public record GetPayrollPeriodDetailsV3Query(int PayPeriodId) : IRequest<List<PayrollPeriodDetailV3Dto>>;
    public record GeneratePayrollPeriodDetailsV3Query(int AtnTypeId, DateTime FromDate, int NoOfPeriod) : IRequest<List<PayrollPeriodDetailV3Dto>>;
    public record GetNextPayrollRegisterDateV3Query(int PayrollRegisterId) : IRequest<NextPayrollRegisterDateV3Dto?>;
}
