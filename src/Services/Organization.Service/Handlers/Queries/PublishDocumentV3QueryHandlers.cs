using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public class PublishDocumentV3QueryHandlers : 
        IRequestHandler<GetPublishDocumentsV3Query, PaginationResponse<PublishDocumentV3Dto>>,
        IRequestHandler<GetPublishDocumentByIdV3Query, PublishDocumentV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public PublishDocumentV3QueryHandlers(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<PublishDocumentV3Dto>> Handle(GetPublishDocumentsV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.PublishDocumentHdrLegacy
                .AsNoTracking()
                .Include(h => h.SMPublishdocumentdtls)
                .Where(h => h.clientid == request.ClientId && !h.utblock);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(h => h.stxt.Contains(request.SearchTerm) || h.code.Contains(request.SearchTerm));
            }

            if (request.DocumentTypeId.HasValue)
            {
                query = query.Where(h => h.doctypeid == request.DocumentTypeId);
            }

            if (request.ValidFrom.HasValue)
            {
                query = query.Where(h => h.validfrom >= request.ValidFrom);
            }

            if (request.ValidTill.HasValue)
            {
                query = query.Where(h => h.validtill <= request.ValidTill);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(h => h.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(h => new PublishDocumentV3Dto
                {
                    Id = h.Id,
                    Code = h.code,
                    SharedWithId = h.lovsharedid,
                    SharedWithName = _context.Lovs.Where(l => l.Id == h.lovsharedid).Select(l => l.stxt).FirstOrDefault() ?? "",
                    DocumentTypeId = h.doctypeid,
                    DocumentTypeName = _context.Lovs.Where(l => l.Id == h.doctypeid).Select(l => l.stxt).FirstOrDefault() ?? "",
                    PublishDocument = h.stxt,
                    PublishDate = h.publishdate,
                    ValidFrom = h.validfrom,
                    ValidTill = h.validtill,
                    DocumentCategoryId = h.lovdoccategoryid,
                    DocumentCategoryName = _context.Lovs.Where(l => l.Id == h.lovdoccategoryid).Select(l => l.stxt).FirstOrDefault() ?? "",
                    CourseId = h.courseid,
                    SubjectId = h.subjectid,
                    AcademicYearId = h.acdyearid
                })
                .ToListAsync(cancellationToken);

            return new PaginationResponse<PublishDocumentV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

        }

        public async Task<PublishDocumentV3Dto> Handle(GetPublishDocumentByIdV3Query request, CancellationToken cancellationToken)
        {
            var h = await _context.PublishDocumentHdrLegacy
                .Include(h => h.SMPublishdocumentdtls)
                .FirstOrDefaultAsync(h => h.Id == request.Id && h.clientid == request.ClientId && !h.utblock, cancellationToken);

            if (h == null)
            {
                throw new KeyNotFoundException($"Publish Document with ID {request.Id} not found.");
            }

            return new PublishDocumentV3Dto
            {
                Id = h.Id,
                Code = h.code,
                SharedWithId = h.lovsharedid,
                SharedWithName = _context.Lovs.Where(l => l.Id == h.lovsharedid).Select(l => l.stxt).FirstOrDefault() ?? "",
                DocumentTypeId = h.doctypeid,
                DocumentTypeName = _context.Lovs.Where(l => l.Id == h.doctypeid).Select(l => l.stxt).FirstOrDefault() ?? "",
                PublishDocument = h.stxt,
                PublishDate = h.publishdate,
                ValidFrom = h.validfrom,
                ValidTill = h.validtill,
                DocumentCategoryId = h.lovdoccategoryid,
                DocumentCategoryName = _context.Lovs.Where(l => l.Id == h.lovdoccategoryid).Select(l => l.stxt).FirstOrDefault() ?? "",
                CourseId = h.courseid,
                SubjectId = h.subjectid,
                AcademicYearId = h.acdyearid,
                Details = h.SMPublishdocumentdtls.Select(d => new PublishDocumentDtlV3Dto
                {
                    Id = d.Id,
                    PublishId = d.publishid,
                    ShareTypeId = d.lovshareid,
                    ShareTypeName = _context.Lovs.Where(l => l.Id == d.lovshareid).Select(l => l.stxt).FirstOrDefault() ?? "",
                    ClientId = d.pclientid,
                    EntityId = d.pentityid,
                    RegionId = d.regionid,
                    CampusLevelId = d.camplvelid,
                    CourseId = d.courseid
                }).ToList()
            };
        }
    }
}
