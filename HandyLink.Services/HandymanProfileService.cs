using Azure;
using Azure.Core;
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
using System.Linq.Dynamic.Core;


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

        public virtual async Task<PageResult<HandymanProfileListResponse>> GetAllAsync(HandymanProfileSearchObject? searchObject = null)
        {
            IEnumerable<HandymanProfile> query = _dbContext.Set<HandymanProfile>();

            query = IncludeRelatedEntities(query.AsQueryable(), searchObject);
            query = ApplyFilters(query, searchObject);

            int? totalCount = null;

            if (searchObject != null)
            {
                if (searchObject.IncludeTotalCount)
                {
                    totalCount = query.Count();
                }
                if (!string.IsNullOrWhiteSpace(searchObject.SortBy))
                {
                    query = query.AsQueryable().OrderBy(searchObject.SortBy);
                }
                query = query.Skip((searchObject.Page - 1) * searchObject.PageSize);
                query = query.Take(searchObject.PageSize);

            }

            var list = query.Select(item => _mapper.Map<HandymanProfileListResponse>(item)).ToList();

            var pageResult = new PageResult<HandymanProfileListResponse>
            {
                Items = list,
                TotalCount = totalCount,
            };

            return await Task.FromResult(pageResult);



        }

        public async Task<HandymanProfileDetailsResponse> GetByIdAsync(int id)
        {
            var query = _dbContext.HandymanProfiles.AsQueryable();
            query = IncludeRelatedEntities(query, null);

            var profile = await query.FirstOrDefaultAsync(x => x.Id == id);

            if (profile == null)
                throw new HandyLinkNotFoundException($"HandymanProfile with id {id} not found.");
            
            var response = _mapper.Map<HandymanProfileDetailsResponse>(profile);

            var application = await _dbContext.HandymanApplications.Where(x => x.UserId == profile.UserId && x.Status == HandymanApplicationStatus.Approved).OrderByDescending(x => x.CreatedAtUtc).FirstOrDefaultAsync();
            if (application == null)
            {
                throw new HandyLinkNotFoundException($"HandymanApplication not found for HandymanProfile with id: {id}.");
            }

            response.JobsCompleted = profile.Jobs.Where(x=>x.JobStatus.Code=="COMPLETED").Count();
            response.AverageRating = profile.Reviews.Count==0?0:profile.Reviews.Average(r => r.Rating);
            response.ReviewsCount = profile.Reviews.Count;

            return response;
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

            var application = await _dbContext.HandymanApplications
                    .Include(x => x.HandymanApplicationServiceCategories)
                    .Where(x => x.UserId == request.UserId && x.Status == HandymanApplicationStatus.Approved).
                    OrderByDescending(x => x.CreatedAtUtc)
                    .FirstOrDefaultAsync();
            if (application == null) {
                throw new HandyLinkNotFoundException($"User does not have an approved HandymanApplication.");
            }

            entity.HandymanServiceCategories = application.HandymanApplicationServiceCategories
                .Select(x => new HandymanServiceCategory
                {
                    ServiceCategoryId = x.ServiceCategoryId
                }).ToList();
            entity.ExperienceYears = application.ExperienceYears;


            _dbContext.HandymanProfiles.Add(entity);
            await _dbContext.SaveChangesAsync();        

            return await Task.FromResult(_mapper.Map<HandymanProfileDetailsResponse>(entity));
        }



        private IEnumerable<HandymanProfile> ApplyFilters(IEnumerable<HandymanProfile> query, HandymanProfileSearchObject? searchObject)
        {
            if (searchObject?.SearchTerm != null)
            {
                var normalized = searchObject.SearchTerm.Trim().ToLower();
                query = query
                    .Where(x => (x.User.FirstName + " " + x.User.LastName).ToLower().Contains(normalized));
            }
            if (searchObject?.CityId != null)
            {
                query = query.Where(x => x.User.CityId== searchObject.CityId);
            }
            if (searchObject?.ServiceCategoryId != null)
            {
                query = query.Where(x => x.HandymanServiceCategories.Any(sc=>sc.ServiceCategoryId==searchObject.ServiceCategoryId));
            }
            if (searchObject?.MinExperienceYears != null)
            {
                query = query.Where(x => x.ExperienceYears>=searchObject.MinExperienceYears);
            }
            return query;
        }

        private IQueryable<HandymanProfile> IncludeRelatedEntities(IQueryable<HandymanProfile> query, HandymanProfileSearchObject? searchObject)
        {

            return query
                .Include(x => x.User)
                    .ThenInclude(x => x.City)
                .Include(x => x.HandymanServiceCategories)
                    .ThenInclude(x => x.ServiceCategory)

                .Include(x => x.HandymanWorkPhotos)

                .Include(x => x.Reviews)
                .Include(x => x.User).ThenInclude(x => x.UserStatus)
                .Include(x => x.Jobs).ThenInclude(x=>x.JobStatus);
            

        }

    }
}
