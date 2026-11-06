using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class RatingModelCommandHandlers :
        IRequestHandler<CreateRatingModelV3Command, RatingModelV3Dto>,
        IRequestHandler<UpdateRatingModelV3Command, bool>,
        IRequestHandler<DeleteRatingModelV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public RatingModelCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<RatingModelV3Dto> Handle(CreateRatingModelV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            ValidateRatingDetails(request.Items);

            if (await _context.SkillRatings.AnyAsync(x => x.Code == request.Code && x.EntityId == command.EntityId && x.ClientId == command.ClientId, cancellationToken))
            {
                throw new InvalidOperationException($"Rating Model with code '{request.Code}' already exists.");
            }
            if (await _context.SkillRatings.AnyAsync(x => x.Stxt == request.RatingModel && x.EntityId == command.EntityId && x.ClientId == command.ClientId, cancellationToken))
            {
                throw new InvalidOperationException($"Rating Model '{request.RatingModel}' already exists.");
            }

            var nextId = (await _context.SkillRatings.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var entity = new ERSkillratinghdrLegacy
            {
                Id = nextId,
                Code = request.Code.Trim(),
                Stxt = request.RatingModel.Trim(),
                EntityId = command.EntityId,
                ClientId = command.ClientId,
                UtBlock = !request.IsActive
            };

            var nextDetailId = (await _context.SkillRatingDetails.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            if (request.Items != null && request.Items.Any())
            {
                int lineIndex = 1;
                foreach (var item in request.Items)
                {
                    entity.Details.Add(new ERSkillratingdtlLegacy
                    {
                        Id = nextDetailId++,
                        RatingId = entity.Id,
                        LineIndex = lineIndex++,
                        Code = item.Code?.Trim(),
                        Key = item.Key?.Trim(),
                        Stxt = item.Description?.Trim(),
                        Factor = item.Factor,
                        UtBlock = false
                    });
                }
            }

            _context.SkillRatings.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new RatingModelV3Dto(
                entity.Id,
                entity.Code ?? string.Empty,
                entity.Stxt ?? string.Empty,
                !(entity.UtBlock ?? false),
                entity.Details.Select(d => new RatingModelItemV3Dto(
                    d.Id,
                    d.Code ?? string.Empty,
                    d.Key ?? string.Empty,
                    d.Stxt ?? string.Empty,
                    d.Factor ?? 0
                )).ToList()
            );
        }

        public async Task<bool> Handle(UpdateRatingModelV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            ValidateRatingDetails(request.Items?.Where(x => !x.Delete));
            var entity = await _context.SkillRatings
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == command.ClientId && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            if (await _context.SkillRatings.AnyAsync(x => x.Code == request.Code && x.EntityId == command.EntityId && x.ClientId == command.ClientId && x.Id != request.Id, cancellationToken))
            {
                throw new InvalidOperationException($"Another Rating Model with code '{request.Code}' already exists.");
            }
            if (await _context.SkillRatings.AnyAsync(x => x.Stxt == request.RatingModel && x.EntityId == command.EntityId && x.ClientId == command.ClientId && x.Id != request.Id, cancellationToken))
            {
                throw new InvalidOperationException($"Another Rating Model '{request.RatingModel}' already exists.");
            }

            entity.Code = request.Code.Trim();
            entity.Stxt = request.RatingModel.Trim();
            entity.UtBlock = !request.IsActive;

            var nextDetailId = (await _context.SkillRatingDetails.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            if (request.Items != null)
            {
                int lineIndex = 1;

                var existingDetailsMap = entity.Details.ToDictionary(d => d.Id);

                foreach (var itemReq in request.Items)
                {
                    if (itemReq.Id > 0 && existingDetailsMap.TryGetValue(itemReq.Id, out var existingDetail))
                    {
                        if (itemReq.Delete)
                        {
                            entity.Details.Remove(existingDetail);
                            _context.SkillRatingDetails.Remove(existingDetail);
                        }
                        else
                        {
                            existingDetail.LineIndex = lineIndex++;
                            existingDetail.Code = itemReq.Code?.Trim();
                            existingDetail.Key = itemReq.Key?.Trim();
                            existingDetail.Stxt = itemReq.Description?.Trim();
                            existingDetail.Factor = itemReq.Factor;
                        }
                    }
                    else if (!itemReq.Delete)
                    {
                        var newDetail = new ERSkillratingdtlLegacy
                        {
                            Id = nextDetailId++,
                            RatingId = entity.Id,
                            LineIndex = lineIndex++,
                            Code = itemReq.Code?.Trim(),
                            Key = itemReq.Key?.Trim(),
                            Stxt = itemReq.Description?.Trim(),
                            Factor = itemReq.Factor,
                            UtBlock = false
                        };
                        entity.Details.Add(newDetail);
                    }
                }

                // If any existing item was removed from request, we should hard-delete it
                var reqItemIds = request.Items.Where(x => x.Id > 0 && !x.Delete).Select(x => x.Id).ToHashSet();
                var itemsToRemove = entity.Details.Where(d => d.Id > 0 && !reqItemIds.Contains(d.Id)).ToList();
                foreach (var detail in itemsToRemove)
                {
                    entity.Details.Remove(detail);
                    _context.SkillRatingDetails.Remove(detail);
                }
            }
            else
            {
                // Hard delete all
                _context.SkillRatingDetails.RemoveRange(entity.Details);
                entity.Details.Clear();
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static void ValidateRatingDetails<T>(IEnumerable<T>? details)
        {
            if (details == null || !details.Any())
                throw new InvalidOperationException("At least one Rating Detail is required.");
        }

        public async Task<bool> Handle(DeleteRatingModelV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.SkillRatings
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == command.Id && x.ClientId == command.ClientId && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            _context.SkillRatingDetails.RemoveRange(entity.Details);
            _context.SkillRatings.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
