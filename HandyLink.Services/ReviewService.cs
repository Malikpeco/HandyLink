using FluentValidation;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services
{
    public class ReviewService : IReviewService
    {

        private readonly HandyLinkDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<ReviewInsertRequest> _insertValidator;

        public ReviewService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<ReviewInsertRequest> insertValidator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _insertValidator = insertValidator;
        }

        public async Task<ReviewResponse> CreateReviewAsync(int jobId, ReviewInsertRequest request)
        {
            var validationResult = await _insertValidator.ValidateAsync(request);
            if (validationResult.IsValid == false)
            {
                throw new HandyLinkValidationException(validationResult.Errors);
            }



            var job = await _dbContext.Jobs.Include(x=>x.JobStatus).FirstOrDefaultAsync(x => x.Id == jobId);

            if (job == null)
                throw new HandyLinkNotFoundException($"Job with Id {jobId} not found.");

            var handymanProfileId = job.HandymanProfileId;

            if (handymanProfileId == null)
                throw new HandyLinkBusinessRuleException($"Job must have an assigned handyman.");

            if (!await _dbContext.ClientProfiles.AnyAsync(x => x.Id == request.ClientProfileId))
                throw new HandyLinkNotFoundException($"ClientProfile with Id {request.ClientProfileId} not found.");
            
            if (job.ClientProfileId != request.ClientProfileId)
                throw new HandyLinkForbiddenException("Only the assigned client can create a review for this job.");

            if (job.JobStatus.Code != "COMPLETED")
                throw new HandyLinkBusinessRuleException("A review can be made only for jobs with status COMPLETED.");

            if (await _dbContext.Reviews.AnyAsync(x => x.JobId == jobId))
                throw new HandyLinkBusinessRuleException("A review already exists for this job.");

            var entity = _mapper.Map<Review>(request);
            entity.JobId = jobId;
            entity.HandymanProfileId = handymanProfileId.Value;

            _dbContext.Reviews.Add(entity);
            await _dbContext.SaveChangesAsync();

            var createdJob = await IncludeRelatedEntitiesAsync(entity);
            return _mapper.Map<ReviewResponse>(createdJob);
        }


        public async Task<PageResult<ReviewResponse>> GetHandymanReviewsAsync(int handymanProfileId)
        {
            var handymanProfile = await _dbContext.HandymanProfiles.FirstOrDefaultAsync(x => x.Id == handymanProfileId);
            if (handymanProfile == null)
                throw new HandyLinkNotFoundException($"HandymanProfile with id {handymanProfileId} not found.");

            var query = _dbContext.Reviews.Where(x => x.HandymanProfileId == handymanProfileId).AsQueryable();

            query = IncludeRelatedEntitiesQuery(query);


            int? totalCount = null;

            var list = query.Select(item => _mapper.Map<ReviewResponse>(item)).ToList();

            var pageResult = new PageResult<ReviewResponse>
            {
                Items = list,
                TotalCount = totalCount,
            };

            return await Task.FromResult(pageResult);
        }






        public async Task<PageResult<ReviewResponse>> GetAdminReviewsAsync(ReviewSearchObject? searchObject = null)
        {
            var query = _dbContext.Reviews.AsQueryable();

            query = IncludeRelatedEntitiesQuery(query);

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

            var list = query.Select(item => _mapper.Map<ReviewResponse>(item)).ToList();

            var pageResult = new PageResult<ReviewResponse>
            {
                Items = list,
                TotalCount = totalCount,
            };

            return await Task.FromResult(pageResult);
        }






        public async Task DeleteReviewAsync(int id)
        {
            var review = _dbContext.Set<Review>().Find(id);

            if (review == null)
                throw new HandyLinkNotFoundException($"Review with id {id} not found.");

            _dbContext.Set<Review>().Remove(review);
            await _dbContext.SaveChangesAsync();
        }




        private IQueryable<Review> ApplyFilters(IQueryable<Review> query, ReviewSearchObject? searchObject)
        {
            if (searchObject?.SearchTerm != null)
            {
                var normalized = searchObject.SearchTerm.Trim().ToLower();

                query = query.Where(x =>
                    x.Job.Title.ToLower().Contains(normalized) ||
                    (x.ClientProfile.User.FirstName + " " + x.ClientProfile.User.LastName).ToLower().Contains(normalized) ||
                    (x.HandymanProfile.User.FirstName + " " + x.HandymanProfile.User.LastName).ToLower().Contains(normalized) ||
                    x.Comment!=null && x.Comment.ToLower().Contains(normalized));
            }

            if (searchObject?.MinRating != null)
                query = query.Where(x => x.Rating >= searchObject.MinRating);
            
            if (searchObject?.MaxRating != null)
                query = query.Where(x => x.Rating <= searchObject.MaxRating);

            if (searchObject?.CreatedFromUtc != null)
                query = query.Where(x => x.CreatedAtUtc >= searchObject.CreatedFromUtc);

            if (searchObject?.CreatedToUtc != null)
                query = query.Where(x => x.CreatedAtUtc <= searchObject.CreatedToUtc);

            return query;
        }













        private async Task<Review> IncludeRelatedEntitiesAsync(Review entity)
        {
            return await _dbContext.Reviews
                .Include(x => x.ClientProfile)
                    .ThenInclude(x => x.User)
                .Include(x => x.HandymanProfile!)
                    .ThenInclude(x => x.User)
                .Include(x => x.Job)
                    .ThenInclude(x=>x.JobStatus)
                .FirstOrDefaultAsync(x => x.Id == entity.Id)
                ?? throw new HandyLinkNotFoundException($"Review with id {entity.Id} not found.");
        }
        private IQueryable<Review> IncludeRelatedEntitiesQuery(IQueryable<Review> query)
        {
            return query
                .Include(x => x.ClientProfile)
                    .ThenInclude(x => x.User)
                .Include(x => x.HandymanProfile!)
                    .ThenInclude(x => x.User)
                .Include(x => x.Job)
                    .ThenInclude(x => x.JobStatus);
        }
    }
}
