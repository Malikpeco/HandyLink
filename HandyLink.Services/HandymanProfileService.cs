using FluentValidation;
using HandyLink.Model.Database.Enums;
using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Database;
using HandyLink.Services.Database.Entities;
using HandyLink.Services.Exceptions;
using HandyLink.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services
{
    public class HandymanProfileService : IHandymanProfileService
    {
        private readonly HandyLinkDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<HandymanProfileInsertRequest> _insertValidator;
        
        public HandymanProfileService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<HandymanProfileInsertRequest> insertValidator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _insertValidator = insertValidator;
        }


        public async Task<HandymanProfileDetailsResponse> GetByIdAsync(int id)
        {
            var query = _dbContext.HandymanProfiles.AsQueryable();
            query = IncludeRelatedEntities(query, null);

            var profile = await query.FirstOrDefaultAsync(x => x.Id == id);

            if (profile == null)
                throw new HandyLinkNotFoundException($"HandymanProfile with id {id} not found.");

            return _mapper.Map<HandymanProfileDetailsResponse>(profile);
        }


        public async Task<HandymanProfileDetailsResponse> InsertAsync(HandymanProfileInsertRequest request)
        {
            var validationResult = await _insertValidator.ValidateAsync(request);
            if (validationResult.IsValid == false)
            {
                throw new HandyLinkValidationException(validationResult.Errors);
            }
            if(await _dbContext.HandymanProfiles.AnyAsync(x=>x.UserId==request.UserId))
            {
                throw new HandyLinkBusinessRuleException($"User with id: {request.UserId} already has a HandymanProfile.");
            }

            var entity = _mapper.Map<HandymanProfile>(request);

            var application = await _dbContext.HandymanApplications.Where(x => x.UserId == request.UserId && x.Status == HandymanApplicationStatus.Approved).OrderByDescending(x => x.CreatedAtUtc).FirstOrDefaultAsync();
            if (application == null) {
                throw new HandyLinkNotFoundException($"User does not have an approved HandymanApplication.");
            }

            entity.HandymanServiceCategories = application.HandymanApplicationServiceCategories
                .Select(x => new HandymanServiceCategory
                {
                    ServiceCategoryId = x.ServiceCategoryId
                }).ToList();

            _dbContext.HandymanProfiles.Add(entity);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(_mapper.Map<HandymanProfileDetailsResponse>(entity));
        }




        private IQueryable<HandymanProfile> IncludeRelatedEntities(IQueryable<HandymanProfile> query, HandymanProfileSearchObject? searchObject)
        {

            return query
                .Include(x => x.User)
                    .ThenInclude(x => x.City)
                .Include(x => x.HandymanServiceCategories)
                    .ThenInclude(x => x.ServiceCategory)

                .Include(x => x.HandymanWorkPhotos)

                .Include(x => x.Reviews);

        }

    }
}
