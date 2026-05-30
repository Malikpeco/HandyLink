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
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services
{
    public class JobService : IJobService
    {
        private readonly HandyLinkDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<JobInsertRequest> _insertValidator;

        public JobService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<JobInsertRequest> insertValidator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _insertValidator = insertValidator;
        }



        public async Task<JobDetailsResponse> CreateJobAsync(JobInsertRequest request)
        {
            var validationResult = await _insertValidator.ValidateAsync(request);
            if (validationResult.IsValid == false)
            {
                throw new HandyLinkValidationException(validationResult.Errors);
            }


            if (!await _dbContext.ClientProfiles.AnyAsync(x => x.Id == request.ClientProfileId))
                throw new HandyLinkNotFoundException($"ClientProfile with id {request.ClientProfileId} not found.");

            if (request.HandymanProfileId != null && !await _dbContext.HandymanProfiles.AnyAsync(x => x.Id == request.HandymanProfileId))
                throw new HandyLinkNotFoundException($"HandymanProfile with id {request.HandymanProfileId} not found.");

            if (!await _dbContext.ServiceCategories.AnyAsync(x => x.Id == request.ServiceCategoryId))
                throw new HandyLinkNotFoundException($"ServiceCategory with id {request.ServiceCategoryId} not found.");

            if (!await _dbContext.Cities.AnyAsync(x => x.Id == request.CityId))
                throw new HandyLinkNotFoundException($"City with id {request.CityId} not found.");


            if (request.HandymanProfileId == null && request.JobCreationType == JobCreationType.DirectProposal)
                throw new HandyLinkBusinessRuleException($"Jobs with JobCreationType.DirectProposal must have a HandymanProfileId.");

            if (request.HandymanProfileId != null && request.JobCreationType == JobCreationType.PublicRequest)
                throw new HandyLinkBusinessRuleException($"Jobs with JobCreationType.PublicRequest cannot have a HandymanProfileId.");

            if (request.JobCreationType == JobCreationType.DirectProposal && request.Address == null)
                throw new HandyLinkBusinessRuleException($"Address is required for Jobs with JobCreationType.DirectProposal.");
            if (request.JobCreationType == JobCreationType.PublicRequest && request.Address != null)
                throw new HandyLinkBusinessRuleException($"Address must be empty for Jobs with JobCreationType.PublicRequest.");


            if (request.InitialPriceOnArrangement && request.InitialPrice != null)
                throw new HandyLinkBusinessRuleException("InitialPrice must be empty when price is on arrangement.");

            if (!request.InitialPriceOnArrangement && request.InitialPrice == null)
                throw new HandyLinkBusinessRuleException("InitialPrice is required when price is not on arrangement.");

            var entity = _mapper.Map<Job>(request);

            var pendingStatus = await _dbContext.JobStatuses.FirstOrDefaultAsync(x => x.Code == "PENDING");
            if (pendingStatus == null)
                throw new HandyLinkNotFoundException("JobStatus 'PENDING' not found, it must exist.");

            entity.JobStatusId = pendingStatus.Id;

            entity.CurrentPrice = request.InitialPrice;
            entity.CurrentPriceOnArrangement = request.InitialPriceOnArrangement;

            if (request.InitialScheduledAtUtc == default)
                throw new HandyLinkBusinessRuleException("InitalScheduledAt is required.");

            entity.CurrentTimeFlexible = request.InitialTimeFlexible;

            entity.InitialScheduledAtUtc = request.InitialTimeFlexible
                ? request.InitialScheduledAtUtc.Date
                : request.InitialScheduledAtUtc;

            entity.CurrentScheduledAtUtc = entity.InitialScheduledAtUtc;


            _dbContext.Jobs.Add(entity);
            await _dbContext.SaveChangesAsync();

            var createdJob = await IncludeRelatedEntitiesAsync(entity);
            return _mapper.Map<JobDetailsResponse>(createdJob);
        }



        public async Task<JobDetailsResponse> GetByIdAsync(int id)
        {
            var query = _dbContext.Jobs.AsQueryable();
            query = IncludeRelatedEntitiesList(query);

            var entity = await query.FirstOrDefaultAsync(e => e.Id == id);

            if (entity == null)
            {
                throw new HandyLinkNotFoundException($"Job with id {id} not found.");
            }

            return await Task.FromResult(_mapper.Map<JobDetailsResponse>(entity));
        }


        public async Task<JobDetailsResponse> AddCompletionMarkAsync(JobMarkRequest request)
        {
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(x => x.Id == request.JobId);
            if (job == null)
                throw new HandyLinkNotFoundException($"Job with id {request.JobId} not found.");

            job = await IncludeRelatedEntitiesAsync(job);

            if (job.JobStatus.Code != "CONFIRMED")
                throw new HandyLinkBusinessRuleException($"JobStatus must be CONFIRMED for a completed-mark to be added.");

            if (job.HandymanProfileId == null || job.HandymanProfile==null)
                throw new HandyLinkBusinessRuleException("Job must have a HandymanProfile.");

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.MarkedByUserId);
            if (user == null)
                throw new HandyLinkNotFoundException($"User with id {request.MarkedByUserId} not found.");

            var clientUserId = job.ClientProfile.UserId;
            var handymanUserId = job.HandymanProfile.UserId;

            if (request.MarkedByUserId != clientUserId && request.MarkedByUserId != handymanUserId)
                throw new HandyLinkForbiddenException("Only the client or the assigned handyman can mark this job as completed.");

            if (job.JobCompletionMarks.Any(x => x.MarkedByUserId == request.MarkedByUserId))
                throw new HandyLinkBusinessRuleException("User already marked this job as completed.");

            var existingCancellationMark = job.JobCancellationMarks.FirstOrDefault(x=>x.MarkedByUserId==request.MarkedByUserId);
            if (existingCancellationMark != null)
            {
                _dbContext.JobCancellationMarks.Remove(existingCancellationMark);
                job.JobCancellationMarks.Remove(existingCancellationMark);
            } 
                
            job.JobCompletionMarks.Add(new JobCompletionMark
            {
                JobId = request.JobId,
                MarkedByUserId = request.MarkedByUserId
            });

            var hasClientMark = job.JobCompletionMarks.Any(x => x.MarkedByUserId == clientUserId);
            var hasHandymanMark = job.JobCompletionMarks.Any(x => x.MarkedByUserId == handymanUserId);
            if(hasClientMark && hasHandymanMark)
            {
                var completedStatus = await _dbContext.JobStatuses.FirstOrDefaultAsync(x => x.Code == "COMPLETED");

                if (completedStatus == null)
                    throw new HandyLinkNotFoundException("Job status COMPLETED does not exist.");

                job.JobStatusId = completedStatus.Id;
                job.JobStatus = completedStatus;
                job.CompletedAtUtc = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<JobDetailsResponse>(job);

        }



        public async Task<JobDetailsResponse> AddCancellationMarkAsync(JobMarkRequest request)
        {
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(x => x.Id == request.JobId);
            if (job == null)
                throw new HandyLinkNotFoundException($"Job with id {request.JobId} not found.");

            job = await IncludeRelatedEntitiesAsync(job);

            if (job.JobStatus.Code != "CONFIRMED")
                throw new HandyLinkBusinessRuleException($"JobStatus must be CONFIRMED for a cancelled-mark to be added.");

            if (job.HandymanProfileId == null || job.HandymanProfile == null)
                throw new HandyLinkBusinessRuleException("Job must have a HandymanProfile.");

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.MarkedByUserId);
            if (user == null)
                throw new HandyLinkNotFoundException($"User with id {request.MarkedByUserId} not found.");

            var clientUserId = job.ClientProfile.UserId;
            var handymanUserId = job.HandymanProfile.UserId;

            if (request.MarkedByUserId != clientUserId && request.MarkedByUserId != handymanUserId)
                throw new HandyLinkForbiddenException("Only the client or the assigned handyman can mark this job as cancelled.");

            if (job.JobCancellationMarks.Any(x => x.MarkedByUserId == request.MarkedByUserId))
                throw new HandyLinkBusinessRuleException("User already marked this job as cancelled.");

            var existingCompletionMark = job.JobCompletionMarks.FirstOrDefault(x => x.MarkedByUserId == request.MarkedByUserId);
            if (existingCompletionMark != null)
            {
                _dbContext.JobCompletionMarks.Remove(existingCompletionMark);
                job.JobCompletionMarks.Remove(existingCompletionMark);
            }

            job.JobCancellationMarks.Add(new JobCancellationMark
            {
                JobId = request.JobId,
                MarkedByUserId = request.MarkedByUserId
            });

            var hasClientMark = job.JobCancellationMarks.Any(x => x.MarkedByUserId == clientUserId);
            var hasHandymanMark = job.JobCancellationMarks.Any(x => x.MarkedByUserId == handymanUserId);
            if (hasClientMark && hasHandymanMark)
            {
                var cancelledStatus = await _dbContext.JobStatuses.FirstOrDefaultAsync(x => x.Code == "CANCELLED");

                if (cancelledStatus == null)
                    throw new HandyLinkNotFoundException("Job status CANCELLED does not exist.");

                job.JobStatusId = cancelledStatus.Id;
                job.JobStatus = cancelledStatus;
                job.CancelledAtUtc = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<JobDetailsResponse>(job);

        }



        public async Task<JobDetailsResponse> AcceptJobAsync(JobAcceptRequest request)
        {
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(x => x.Id == request.JobId);
            if (job == null)
                throw new HandyLinkNotFoundException($"Job with id {request.JobId} not found.");
            job = await IncludeRelatedEntitiesAsync(job);

            var handyman = await _dbContext.HandymanProfiles.FirstOrDefaultAsync(x=>x.Id== request.HandymanProfileId);
            if (handyman == null)
                throw new HandyLinkNotFoundException($"HandymanProfile with id {request.HandymanProfileId} not found.");


            if (job.JobStatus.Code != "PENDING")
                throw new HandyLinkBusinessRuleException("Only PENDING jobs can be accepted.");

            if (job.JobCreationType == JobCreationType.DirectProposal)
            {
                if (job.HandymanProfileId != request.HandymanProfileId)
                    throw new HandyLinkBusinessRuleException($"This job is assigned to another handyman.");
            }

            var confirmedStatus = await _dbContext.JobStatuses.FirstOrDefaultAsync( x => x.Code=="CONFIRMED");
            if (confirmedStatus == null)
                throw new HandyLinkNotFoundException("Job status CONFIRMED does not exist.");

            job.HandymanProfileId = request.HandymanProfileId;
            job.JobStatusId = confirmedStatus.Id;

            await _dbContext.SaveChangesAsync();
            return _mapper.Map<JobDetailsResponse>(job);
        }


        public async Task<JobDetailsResponse> DeclineJobAsync(JobDeclineRequest request)
        {
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(x => x.Id == request.JobId);
            if (job == null)
                throw new HandyLinkNotFoundException($"Job with id {request.JobId} not found.");
            job = await IncludeRelatedEntitiesAsync(job);

            var handyman = await _dbContext.HandymanProfiles.FirstOrDefaultAsync(x => x.Id == request.HandymanProfileId);
            if (handyman == null)
                throw new HandyLinkNotFoundException($"HandymanProfile with id {request.HandymanProfileId} not found.");

            if (job.JobCreationType != JobCreationType.DirectProposal)
                throw new HandyLinkBusinessRuleException("Only DirectProposal jobs can be declined.");

            if (job.JobStatus.Code != "PENDING")
                throw new HandyLinkBusinessRuleException("Only PENDING jobs can be declined.");

            if (job.HandymanProfileId != request.HandymanProfileId)
                throw new HandyLinkBusinessRuleException($"This job is assigned to another handyman.");

            var cancelledStatus = await _dbContext.JobStatuses.FirstOrDefaultAsync(x => x.Code == "CANCELLED");
            if (cancelledStatus == null)
                throw new HandyLinkNotFoundException("Job status CANCELLED does not exist.");

            job.HandymanProfileId = request.HandymanProfileId;
            job.JobStatusId = cancelledStatus.Id;

            await _dbContext.SaveChangesAsync();
            return _mapper.Map<JobDetailsResponse>(job);
        }





        private async Task<Job> IncludeRelatedEntitiesAsync(Job entity)
        {
            return await _dbContext.Jobs
                .Include(x => x.ClientProfile)
                    .ThenInclude(x => x.User)
                .Include(x => x.HandymanProfile!)
                    .ThenInclude(x => x.User)
                .Include(x => x.ServiceCategory)
                .Include(x => x.City)
                .Include(x => x.JobStatus)
                .Include(x => x.JobProposals)
                    .ThenInclude(x => x.ProposedByUser)
                .Include(x => x.JobCompletionMarks)
                    .ThenInclude(x => x.MarkedByUser)
                .Include(x => x.JobCancellationMarks)
                    .ThenInclude(x => x.MarkedByUser)
                .Include(x => x.Review)
                .FirstOrDefaultAsync(x => x.Id == entity.Id)
                ?? throw new HandyLinkNotFoundException($"Job with id {entity.Id} not found.");
        }

        private IQueryable<Job> IncludeRelatedEntitiesList(IQueryable<Job> query)
        {

            return query
                .Include(x => x.ClientProfile)
                    .ThenInclude(x => x.User)
                .Include(x => x.HandymanProfile!)
                    .ThenInclude(x => x.User)
                .Include(x => x.ServiceCategory)
                .Include(x => x.City)
                .Include(x => x.JobStatus)
                .Include(x => x.JobProposals)
                    .ThenInclude(x => x.ProposedByUser)
                .Include(x => x.JobCompletionMarks)
                    .ThenInclude(x => x.MarkedByUser)
                .Include(x => x.JobCancellationMarks)
                    .ThenInclude(x => x.MarkedByUser)
                .Include(x => x.Review);

        }

    }
}
