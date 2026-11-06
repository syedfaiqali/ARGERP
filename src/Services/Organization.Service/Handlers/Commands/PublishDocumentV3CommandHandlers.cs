using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Entities;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Handlers.Commands
{
    public class PublishDocumentV3CommandHandlers : 
        IRequestHandler<CreatePublishDocumentV3Command, PublishDocumentV3Dto>,
        IRequestHandler<UpdatePublishDocumentV3Command, PublishDocumentV3Dto>,
        IRequestHandler<UpdateBulkPublishDocumentV3Command, bool>,
        IRequestHandler<DeletePublishDocumentV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public PublishDocumentV3CommandHandlers(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PublishDocumentV3Dto> Handle(CreatePublishDocumentV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(request.ClientId);
            var header = new PublishDocumentHdrLegacy
            {
                code = $"PUB-{DateTime.Now.Ticks.ToString().Substring(10)}", // Simple auto-generated code
                stxt = request.PublishDocument,
                lovsharedid = request.SharedWithId,
                doctypeid = request.DocumentTypeId,
                validfrom = request.ValidFrom,
                validtill = request.ValidTill,
                lovdoccategoryid = request.DocumentCategoryId,
                courseid = request.CourseId,
                subjectid = request.SubjectId,
                acdyearid = request.AcademicYearId,
                clientid = request.ClientId,
                publishdate = DateTime.UtcNow,
                utrecordversion = 1,
                utblock = false
            };

            if (request.Details != null)
            {
                foreach (var detail in request.Details)
                {
                    header.SMPublishdocumentdtls.Add(new PublishDocumentDtlLegacy
                    {
                        lovshareid = detail.ShareTypeId,
                        pclientid = detail.ClientId,
                        pentityid = detail.EntityId,
                        regionid = detail.RegionId,
                        camplvelid = detail.CampusLevelId,
                        courseid = detail.CourseId,
                        utrecordversion = 1,
                        utblock = false
                    });
                }
            }

            _context.PublishDocumentHdrLegacy.Add(header);
            await _context.SaveChangesAsync(cancellationToken);

            return await GetDto(header.Id, request.ClientId);
        }

        public async Task<PublishDocumentV3Dto> Handle(UpdatePublishDocumentV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(request.ClientId);
            var header = await _context.PublishDocumentHdrLegacy
                .Include(h => h.SMPublishdocumentdtls)
                .FirstOrDefaultAsync(h => h.Id == request.Id && h.clientid == request.ClientId, cancellationToken);

            if (header == null)
            {
                throw new KeyNotFoundException($"Publish Document with ID {request.Id} not found.");
            }

            header.stxt = request.PublishDocument;
            header.lovsharedid = request.SharedWithId;
            header.doctypeid = request.DocumentTypeId;
            header.validfrom = request.ValidFrom;
            header.validtill = request.ValidTill;
            header.lovdoccategoryid = request.DocumentCategoryId;
            header.courseid = request.CourseId;
            header.subjectid = request.SubjectId;
            header.acdyearid = request.AcademicYearId;
            header.utrecordversion++;

            // Update details: Remove missing, Update existing, Add new
            var existingDetails = header.SMPublishdocumentdtls.ToList();
            foreach (var d in existingDetails)
            {
                if (!request.Details.Any(rd => rd.Id == d.Id))
                {
                    _context.PublishDocumentDtlLegacy.Remove(d);
                }
            }

            foreach (var rd in request.Details)
            {
                if (rd.Id > 0)
                {
                    var d = existingDetails.FirstOrDefault(ed => ed.Id == rd.Id);
                    if (d != null)
                    {
                        d.lovshareid = rd.ShareTypeId;
                        d.pclientid = rd.ClientId;
                        d.pentityid = rd.EntityId;
                        d.regionid = rd.RegionId;
                        d.camplvelid = rd.CampusLevelId;
                        d.courseid = rd.CourseId;
                        d.utrecordversion++;
                    }
                }
                else
                {
                    header.SMPublishdocumentdtls.Add(new PublishDocumentDtlLegacy
                    {
                        lovshareid = rd.ShareTypeId,
                        pclientid = rd.ClientId,
                        pentityid = rd.EntityId,
                        regionid = rd.RegionId,
                        camplvelid = rd.CampusLevelId,
                        courseid = rd.CourseId,
                        utrecordversion = 1,
                        utblock = false
                    });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return await GetDto(header.Id, request.ClientId);
        }

        public async Task<bool> Handle(UpdateBulkPublishDocumentV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(request.ClientId);
            var headers = await _context.PublishDocumentHdrLegacy
                .Where(h => request.PublishIds.Contains(h.Id) && h.clientid == request.ClientId)
                .ToListAsync(cancellationToken);

            foreach (var h in headers)
            {
                h.validtill = request.NewValidTill;
                h.utrecordversion++;
            }

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> Handle(DeletePublishDocumentV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(request.ClientId);
            var header = await _context.PublishDocumentHdrLegacy
                .FirstOrDefaultAsync(h => h.Id == request.Id && h.clientid == request.ClientId, cancellationToken);

            if (header == null)
            {
                throw new KeyNotFoundException($"Publish Document with ID {request.Id} not found.");
            }

            header.utblock = true;
            header.utrecordversion++;

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        private async Task<PublishDocumentV3Dto> GetDto(int id, int clientId)
        {
            // Simple helper to fetch the DTO after save
            var h = await _context.PublishDocumentHdrLegacy
                .FirstOrDefaultAsync(h => h.Id == id && h.clientid == clientId);

            if (h == null) return null;

            return new PublishDocumentV3Dto
            {
                Id = h.Id,
                Code = h.code,
                PublishDocument = h.stxt
            };
        }
    }
}
