using FluentValidation;
using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
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
    }
}
