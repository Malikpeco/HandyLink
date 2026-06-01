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
    public class JobService : IJobService
    {
        private readonly HandyLinkDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<JobInsertRequest> _insertValidator;
        private readonly IValidator<JobProposalInsertRequest> _proposalInsertValidator;

        public JobService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<JobInsertRequest> insertValidator, IValidator<JobProposalInsertRequest> proposalInsertValidator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _insertValidator = insertValidator;
            _proposalInsertValidator = proposalInsertValidator;
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






        public async Task<PageResult<JobListResponse>> GetAdminJobsAsync(AdminJobSearchObject? searchObject = null)
        {
            var query = _dbContext.Jobs.AsQueryable();

            query = IncludeRelatedEntitiesList(query);

            query = ApplyAdminFilters(query, searchObject);

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

            var list = query.Select(item => _mapper.Map<JobListResponse>(item)).ToList();

            var pageResult = new PageResult<JobListResponse>
            {
                Items = list,
                TotalCount = totalCount,
            };

            return await Task.FromResult(pageResult);
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

            if (job.HandymanProfileId == null || job.HandymanProfile == null)
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

            var existingCancellationMark = job.JobCancellationMarks.FirstOrDefault(x => x.MarkedByUserId == request.MarkedByUserId);
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
            if (hasClientMark && hasHandymanMark)
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








        public async Task<JobDetailsResponse> RemoveCompletionMarkAsync(JobMarkRequest request)
        {
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(x => x.Id == request.JobId);
            if (job == null)
                throw new HandyLinkNotFoundException($"Job with id {request.JobId} not found.");

            job = await IncludeRelatedEntitiesAsync(job);

            if (job.JobStatus.Code != "CONFIRMED")
                throw new HandyLinkBusinessRuleException($"JobStatus must be CONFIRMED for a completed-mark to be removed.");

            if (job.HandymanProfileId == null || job.HandymanProfile == null)
                throw new HandyLinkBusinessRuleException("Job must have a HandymanProfile.");

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.MarkedByUserId);
            if (user == null)
                throw new HandyLinkNotFoundException($"User with id {request.MarkedByUserId} not found.");

            var clientUserId = job.ClientProfile.UserId;
            var handymanUserId = job.HandymanProfile.UserId;

            if (request.MarkedByUserId != clientUserId && request.MarkedByUserId != handymanUserId)
                throw new HandyLinkForbiddenException("Only the client or the assigned handyman can remove a completed-mark from this job.");


            var existingCompletionMark = job.JobCompletionMarks.FirstOrDefault(x => x.MarkedByUserId == request.MarkedByUserId);
            if (existingCompletionMark == null)
                throw new HandyLinkBusinessRuleException("User has not marked this job as completed.");

            _dbContext.JobCompletionMarks.Remove(existingCompletionMark);

            await _dbContext.SaveChangesAsync();

            job = await IncludeRelatedEntitiesAsync(job);

            return _mapper.Map<JobDetailsResponse>(job);

        }







        public async Task<JobDetailsResponse> RemoveCancellationMarkAsync(JobMarkRequest request)
        {
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(x => x.Id == request.JobId);
            if (job == null)
                throw new HandyLinkNotFoundException($"Job with id {request.JobId} not found.");

            job = await IncludeRelatedEntitiesAsync(job);

            if (job.JobStatus.Code != "CONFIRMED")
                throw new HandyLinkBusinessRuleException($"JobStatus must be CONFIRMED for a cancelled-mark to be removed.");

            if (job.HandymanProfileId == null || job.HandymanProfile == null)
                throw new HandyLinkBusinessRuleException("Job must have a HandymanProfile.");

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.MarkedByUserId);
            if (user == null)
                throw new HandyLinkNotFoundException($"User with id {request.MarkedByUserId} not found.");

            var clientUserId = job.ClientProfile.UserId;
            var handymanUserId = job.HandymanProfile.UserId;

            if (request.MarkedByUserId != clientUserId && request.MarkedByUserId != handymanUserId)
                throw new HandyLinkForbiddenException("Only the client or the assigned handyman can remove a cancelled-mark from this job.");


            var existingCancellationMark = job.JobCancellationMarks.FirstOrDefault(x => x.MarkedByUserId == request.MarkedByUserId);
            if (existingCancellationMark == null)
                throw new HandyLinkBusinessRuleException("User has not marked this job as cancelled.");

            _dbContext.JobCancellationMarks.Remove(existingCancellationMark);

            await _dbContext.SaveChangesAsync();

            job = await IncludeRelatedEntitiesAsync(job);

            return _mapper.Map<JobDetailsResponse>(job);

        }









        public async Task<JobDetailsResponse> InstantAcceptDirectProposalAsync(InstantAcceptDirectProposalRequest request)
        {
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(x => x.Id == request.JobId);
            if (job == null)
                throw new HandyLinkNotFoundException($"Job with id {request.JobId} not found.");
            job = await IncludeRelatedEntitiesAsync(job);

            var handyman = await _dbContext.HandymanProfiles.FirstOrDefaultAsync(x => x.Id == request.HandymanProfileId);
            if (handyman == null)
                throw new HandyLinkNotFoundException($"HandymanProfile with id {request.HandymanProfileId} not found.");


            if (job.JobCreationType != JobCreationType.DirectProposal)
                throw new HandyLinkBusinessRuleException("Only DirectProposal jobs can be accepted directly.");

            if (job.JobStatus.Code != "PENDING")
                throw new HandyLinkBusinessRuleException("Only PENDING jobs can be accepted.");

            if (job.JobProposals.Any())
                throw new HandyLinkBusinessRuleException("DirectProposal cannot be accepted immediately after suggested changes have been made.");

            if (job.HandymanProfileId != request.HandymanProfileId)
                throw new HandyLinkBusinessRuleException($"This job is assigned to another handyman.");

            var confirmedStatus = await _dbContext.JobStatuses.FirstOrDefaultAsync(x => x.Code == "CONFIRMED");
            if (confirmedStatus == null)
                throw new HandyLinkNotFoundException("Job status CONFIRMED does not exist.");

            job.JobStatusId = confirmedStatus.Id;
            job.ConfirmedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return _mapper.Map<JobDetailsResponse>(job);
        }









        public async Task<JobDetailsResponse> InstantDeclineDirectProposalAsync(InstantDeclineDirectProposalRequest request)
        {
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(x => x.Id == request.JobId);
            if (job == null)
                throw new HandyLinkNotFoundException($"Job with id {request.JobId} not found.");
            job = await IncludeRelatedEntitiesAsync(job);

            var handyman = await _dbContext.HandymanProfiles.FirstOrDefaultAsync(x => x.Id == request.HandymanProfileId);
            if (handyman == null)
                throw new HandyLinkNotFoundException($"HandymanProfile with id {request.HandymanProfileId} not found.");

            if (job.JobCreationType != JobCreationType.DirectProposal)
                throw new HandyLinkBusinessRuleException("Only DirectProposal jobs can be declined immediately.");

            if (job.JobStatus.Code != "PENDING")
                throw new HandyLinkBusinessRuleException("Only PENDING jobs can be declined.");

            if (job.JobProposals.Any())
                throw new HandyLinkBusinessRuleException("DirectProposal cannot be declined immediately after suggested changes have been made.");

            if (job.HandymanProfileId != request.HandymanProfileId)
                throw new HandyLinkBusinessRuleException($"This job is assigned to another handyman.");

            var cancelledStatus = await _dbContext.JobStatuses.FirstOrDefaultAsync(x => x.Code == "CANCELLED");
            if (cancelledStatus == null)
                throw new HandyLinkNotFoundException("Job status CANCELLED does not exist.");

            job.JobStatusId = cancelledStatus.Id;
            job.CancelledAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return _mapper.Map<JobDetailsResponse>(job);
        }




        public async Task<JobProposalResponse> SuggestChangesAsync(int id, JobProposalInsertRequest request)
        {
            var validationResult = await _proposalInsertValidator.ValidateAsync(request);
            if (validationResult.IsValid == false)
            {
                throw new HandyLinkValidationException(validationResult.Errors);
            }

            var job = await _dbContext.Jobs.FirstOrDefaultAsync(x => x.Id == id);
            if (job == null)
                throw new HandyLinkNotFoundException($"Job with id {id} not found.");
            job = await IncludeRelatedEntitiesAsync(job);

            if (job.JobStatus.Code != "PENDING")
                throw new HandyLinkBusinessRuleException("Job must be PENDING for changes to be suggested.");
            
            if (job.ClientProfile == null)
                throw new HandyLinkBusinessRuleException("Job must have a ClientProfile.");

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.ProposedByUserId);
            if (user == null)
                throw new HandyLinkNotFoundException($"User with id {request.ProposedByUserId} not found.");

            if (request.ProposedPriceOnArrangement && request.ProposedPrice != null)
            {
                throw new HandyLinkBusinessRuleException("ProposedPrice must be empty if ProposedPriceOnArrangement is true.");
            }
            if (!request.ProposedPriceOnArrangement && request.ProposedPrice == null)
            {
                throw new HandyLinkBusinessRuleException("ProposedPrice is required if ProposedPriceOnArrangement is false.");
            }

            if (request.ProposedScheduledAtUtc == default)
                throw new HandyLinkBusinessRuleException("ProposedScheduledAt is required.");




            int proposalHandymanProfileId;

            if (job.JobCreationType == JobCreationType.DirectProposal)
            {
                if (job.HandymanProfileId == null || job.HandymanProfile == null)
                    throw new HandyLinkBusinessRuleException("Job must have a HandymanProfile assigned for changes to be suggested.");
                
                if (request.ProposedByUserId != job.HandymanProfile.UserId && request.ProposedByUserId != job.ClientProfile.UserId)
                    throw new HandyLinkBusinessRuleException("User suggesting changes must be either the assigned handyman or the client.");

                proposalHandymanProfileId = job.HandymanProfileId.Value;
                

                var pendingProposal = job.JobProposals.FirstOrDefault(x=>x.JobProposalStatus == JobProposalStatus.Pending);
                if (pendingProposal == null)
                {
                    if (job.JobProposals.Count == 0 && request.ProposedByUserId != job.HandymanProfile.UserId)
                    {
                        throw new HandyLinkBusinessRuleException("The first suggested changes in a job MUST come from the handyman.");
                    }
                }
                else
                {
                    if (pendingProposal.ProposedByUserId == request.ProposedByUserId)
                        throw new HandyLinkBusinessRuleException("The same user cannot suggest changes twice in a row.");

                    pendingProposal.JobProposalStatus = JobProposalStatus.Superceded;
                }

            }


            else if(job.JobCreationType==JobCreationType.PublicRequest)
            {
                if (job.HandymanProfileId != null)
                {
                    throw new HandyLinkBusinessRuleException("This PublicRequest already has a handyman assigned.");
                }

                if (request.ProposedByUserId == job.ClientProfile.UserId)
                {
                    var handyman = await _dbContext.HandymanProfiles
                        .FirstOrDefaultAsync(x => x.Id == request.HandymanProfileId);

                    if (handyman == null)
                        throw new HandyLinkNotFoundException($"HandymanProfile with id {request.HandymanProfileId} not found.");

                    proposalHandymanProfileId = handyman.Id;
                }
                else
                {
                    var handyman = await _dbContext.HandymanProfiles.FirstOrDefaultAsync(x => x.UserId == request.ProposedByUserId);

                    if (handyman == null)
                        throw new HandyLinkForbiddenException("ProposedByUserId is not an existing UserId of a HandymanProfile.");

                    if (request.HandymanProfileId != handyman.Id)
                        throw new HandyLinkBusinessRuleException("HandymanProfileId does not match the proposing handyman.");

                    proposalHandymanProfileId = handyman.Id;
                }

                var pendingProposal = job.JobProposals
                    .FirstOrDefault(x =>
                        x.HandymanProfileId == proposalHandymanProfileId &&
                        x.JobProposalStatus == JobProposalStatus.Pending);

                if (pendingProposal == null)
                {
                    if (request.ProposedByUserId == job.ClientProfile.UserId)
                    {
                        throw new HandyLinkBusinessRuleException("Client cannot suggest the first changes on a public request since he is the one who posted it.");
                    }
                }
                else
                {
                    if (pendingProposal.ProposedByUserId == request.ProposedByUserId)
                        throw new HandyLinkBusinessRuleException("The same user cannot suggest changes twice in a row.");

                    pendingProposal.JobProposalStatus = JobProposalStatus.Superceded;
                }
            }
            
            else
            {
                throw new HandyLinkBusinessRuleException("Unsupported job creation type.");
            }



            var proposal = _mapper.Map<JobProposal>(request);
            proposal.JobId = job.Id;
            proposal.Job = job;
            
            proposal.ProposedByUserId = user.Id;
            proposal.ProposedByUser = user;

            proposal.HandymanProfileId = proposalHandymanProfileId;

            proposal.JobProposalStatus = JobProposalStatus.Pending;


            proposal.ProposedScheduledAtUtc = request.ProposedTimeFlexible ? request.ProposedScheduledAtUtc.Date : request.ProposedScheduledAtUtc;


            _dbContext.JobProposals.Add(proposal);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<JobProposalResponse>(proposal);
        }









        public async Task<JobDetailsResponse> AcceptSuggestedChangesAsync(JobProposalDecisionRequest request)
        {
            var proposal = await _dbContext.JobProposals.FirstOrDefaultAsync(x => x.Id == request.JobProposalId);
            if (proposal == null)
                throw new HandyLinkNotFoundException($"JobProposal with id {request.JobProposalId} not found.");
            
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (user == null)
                throw new HandyLinkNotFoundException($"User with id {request.UserId} not found.");

            var job = await _dbContext.Jobs.FirstOrDefaultAsync(x => x.Id == proposal.JobId);
            if (job == null)
                throw new HandyLinkNotFoundException($"JobProposal is not assigned to a job.");

            job = await IncludeRelatedEntitiesAsync(job);

            if (job.JobStatus.Code != "PENDING")
                throw new HandyLinkBusinessRuleException("Job must be PENDING for suggested changes to be accepted.");

            if (proposal.JobProposalStatus != JobProposalStatus.Pending)
                throw new HandyLinkBusinessRuleException("Only pending job proposals can be accepted.");

            if (proposal.ProposedByUserId == request.UserId)
                throw new HandyLinkBusinessRuleException("User cannot accept their own suggested changes.");

            var confirmedStatus = await _dbContext.JobStatuses.FirstOrDefaultAsync(x => x.Code == "CONFIRMED");
            if (confirmedStatus == null)
                throw new HandyLinkNotFoundException("JobStatus CONFIRMED not found.");

            
            if (job.JobCreationType == JobCreationType.DirectProposal)
            {
                if (job.HandymanProfileId == null || job.HandymanProfile == null)
                    throw new HandyLinkBusinessRuleException("DirectProposal jobs must have a HandymanProfile.");

                var isClient = request.UserId == job.ClientProfile.UserId;
                var isHandyman = request.UserId == job.HandymanProfile.UserId;

                if (!isClient && !isHandyman)
                    throw new HandyLinkForbiddenException("Only the client or assigned handyman can accept suggested changes.");
            }

            else if (job.JobCreationType == JobCreationType.PublicRequest)
            {
                if (job.HandymanProfileId != null)
                    throw new HandyLinkBusinessRuleException("This public request already has a handyman assigned.");

                var handyman = await _dbContext.HandymanProfiles
                    .FirstOrDefaultAsync(x => x.Id == proposal.HandymanProfileId);

                if (handyman == null)
                    throw new HandyLinkNotFoundException($"HandymanProfile with id {proposal.HandymanProfileId} not found.");

                var isClient = request.UserId == job.ClientProfile.UserId;
                var isProposalHandyman = request.UserId == handyman.UserId;

                if (!isClient && !isProposalHandyman)
                    throw new HandyLinkForbiddenException("Only the client or proposing handyman can accept suggested changes.");

                job.HandymanProfileId = proposal.HandymanProfileId;
            }
            else
            {
                throw new HandyLinkBusinessRuleException("Unsupported job creation type.");
            }

            job.CurrentPrice = proposal.ProposedPrice;
            job.CurrentPriceOnArrangement = proposal.ProposedPriceOnArrangement;
            job.CurrentScheduledAtUtc = proposal.ProposedScheduledAtUtc;
            job.CurrentTimeFlexible = proposal.ProposedTimeFlexible;
            job.JobStatusId = confirmedStatus.Id;
            job.JobStatus = confirmedStatus;
            job.ConfirmedAtUtc = DateTime.UtcNow;
            
            proposal.JobProposalStatus = JobProposalStatus.Accepted;

            if (job.JobCreationType == JobCreationType.PublicRequest)
            {
                foreach (var otherProposal in job.JobProposals
                    .Where(x => x.Id != proposal.Id && x.JobProposalStatus == JobProposalStatus.Pending))
                {
                    otherProposal.JobProposalStatus = JobProposalStatus.Cancelled;
                }
            }

            await _dbContext.SaveChangesAsync();

            job = await IncludeRelatedEntitiesAsync(job);

            return _mapper.Map<JobDetailsResponse>(job);


        }







        public async Task<JobDetailsResponse> DeclineSuggestedChangesAsync(JobProposalDecisionRequest request)
        {
            var proposal = await _dbContext.JobProposals.FirstOrDefaultAsync(x => x.Id == request.JobProposalId);
            if (proposal == null)
                throw new HandyLinkNotFoundException($"JobProposal with id {request.JobProposalId} not found.");
            
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (user == null)
                throw new HandyLinkNotFoundException($"User with id {request.UserId} not found.");

            var job = await _dbContext.Jobs.FirstOrDefaultAsync(x => x.Id == proposal.JobId);
            if (job == null)
                throw new HandyLinkNotFoundException($"JobProposal is not assigned to a job.");

            job = await IncludeRelatedEntitiesAsync(job);

            if (job.JobStatus.Code != "PENDING")
                throw new HandyLinkBusinessRuleException("Job must be PENDING for suggested changes to be declined.");

            if (proposal.JobProposalStatus != JobProposalStatus.Pending)
                throw new HandyLinkBusinessRuleException("Only pending job proposals can be declined.");

            if (proposal.ProposedByUserId == request.UserId)
                throw new HandyLinkBusinessRuleException("User cannot decline their own suggested changes.");


            
            if (job.JobCreationType == JobCreationType.DirectProposal)
            {
                if (job.HandymanProfileId == null || job.HandymanProfile == null)
                    throw new HandyLinkBusinessRuleException("DirectProposal jobs must have a HandymanProfile.");

                var isClient = request.UserId == job.ClientProfile.UserId;
                var isHandyman = request.UserId == job.HandymanProfile.UserId;

                if (!isClient && !isHandyman)
                    throw new HandyLinkForbiddenException("Only the client or assigned handyman can decline suggested changes.");


                var cancelledStatus = await _dbContext.JobStatuses.FirstOrDefaultAsync(x => x.Code == "CANCELLED");
                if (cancelledStatus == null)
                    throw new HandyLinkNotFoundException("JobStatus CANCELLED not found.");

                job.JobStatusId = cancelledStatus.Id;
                job.JobStatus = cancelledStatus;
                job.CancelledAtUtc = DateTime.UtcNow;

            }

            else if (job.JobCreationType == JobCreationType.PublicRequest)
            {
                if (job.HandymanProfileId != null)
                    throw new HandyLinkBusinessRuleException("This public request already has a handyman assigned.");

                var handyman = await _dbContext.HandymanProfiles
                    .FirstOrDefaultAsync(x => x.Id == proposal.HandymanProfileId);

                if (handyman == null)
                    throw new HandyLinkNotFoundException($"HandymanProfile with id {proposal.HandymanProfileId} not found.");

                var isClient = request.UserId == job.ClientProfile.UserId;
                var isProposalHandyman = request.UserId == handyman.UserId;

                if (!isClient && !isProposalHandyman)
                    throw new HandyLinkForbiddenException("Only the client or proposing handyman can decline suggested changes.");
            }
            else
            {
                throw new HandyLinkBusinessRuleException("Unsupported job creation type.");
            }

            
            proposal.JobProposalStatus = JobProposalStatus.Cancelled;


            await _dbContext.SaveChangesAsync();

            job = await IncludeRelatedEntitiesAsync(job);

            return _mapper.Map<JobDetailsResponse>(job);


        }


        



        public async Task<JobDetailsResponse> AddAddressAsync(int jobId, JobAddressUpdateRequest request)
        {
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(x=>x.Id==jobId);
            if (job == null)
                throw new HandyLinkNotFoundException($"Job with id {jobId} not found.");

            job = await IncludeRelatedEntitiesAsync(job);

            if (string.IsNullOrWhiteSpace(request.Address))
                throw new HandyLinkValidationException("Address is required.");

            if (job.JobCreationType != JobCreationType.PublicRequest)
                throw new HandyLinkBusinessRuleException("Job must be a PublicRequest for address to be added this way.");

            if (job.Address != null)
                throw new HandyLinkBusinessRuleException("Job already has an assigned address.");
            
            if (job.JobStatus.Code != "CONFIRMED")
                throw new HandyLinkBusinessRuleException("Job status must be CONFIRMED for an address to be added.");

            var clientProfile = await _dbContext.ClientProfiles.FirstOrDefaultAsync(x => x.Id == request.ClientProfileId);
            if(clientProfile==null)
                throw new HandyLinkNotFoundException($"ClientProfile with id {request.ClientProfileId} not found.");

            if (job.ClientProfileId != clientProfile.Id)
                throw new HandyLinkBusinessRuleException($"ClientProfile with id {clientProfile.Id} is not the ClientProfile assigned to this job.");

            job.Address = request.Address;
            await _dbContext.SaveChangesAsync();

            job = await IncludeRelatedEntitiesAsync(job);

            return _mapper.Map<JobDetailsResponse>(job);
        }












        private IQueryable<Job> ApplyAdminFilters(IQueryable<Job> query, AdminJobSearchObject? searchObject)
        {
            if (searchObject?.SearchTerm != null)
            {
                var normalized = searchObject.SearchTerm.Trim().ToLower();

                query = query.Where(x =>
                    x.Title.ToLower().Contains(normalized) ||
                    x.Description.ToLower().Contains(normalized) ||
                    x.Address != null && x.Address.ToLower().Contains(normalized) ||
                    (x.ClientProfile.User.FirstName + " " + x.ClientProfile.User.LastName).ToLower().Contains(normalized) ||
                    (x.HandymanProfile != null && ((x.HandymanProfile.User.FirstName + " " + x.HandymanProfile.User.LastName).ToLower().Contains(normalized))));
            }
            if (searchObject?.ServiceCategoryId != null)
                query = query.Where(x => x.ServiceCategoryId == searchObject.ServiceCategoryId.Value);

            if (searchObject?.CityId != null)
                query = query.Where(x => x.CityId == searchObject.CityId.Value);

            if (searchObject?.JobStatusId != null)
                query = query.Where(x => x.JobStatusId == searchObject.JobStatusId.Value);

            if (searchObject?.JobCreationType != null)
                query = query.Where(x => x.JobCreationType == searchObject.JobCreationType.Value);

            if (searchObject?.MinCurrentPrice != null)
                query = query.Where(x => x.CurrentPrice >= searchObject.MinCurrentPrice.Value);

            if (searchObject?.MaxCurrentPrice != null)
                query = query.Where(x => x.CurrentPrice <= searchObject.MaxCurrentPrice.Value);

            if (searchObject?.CreatedFromUtc != null)
                query = query.Where(x => x.CreatedAtUtc >= searchObject.CreatedFromUtc.Value);

            if (searchObject?.CreatedToUtc != null)
                query = query.Where(x => x.CreatedAtUtc <= searchObject.CreatedToUtc.Value);

            if (searchObject?.ScheduledFromUtc != null)
                query = query.Where(x => x.CurrentScheduledAtUtc >= searchObject.ScheduledFromUtc.Value);

            if (searchObject?.ScheduledToUtc != null)
                query = query.Where(x => x.CurrentScheduledAtUtc <= searchObject.ScheduledToUtc.Value);

            if (searchObject?.CurrentPriceOnArrangement != null)
                query = query.Where(x => x.CurrentPriceOnArrangement == searchObject.CurrentPriceOnArrangement.Value);

            if (searchObject?.CurrentTimeFlexible != null)
                query = query.Where(x => x.CurrentTimeFlexible == searchObject.CurrentTimeFlexible.Value);

            return query;
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
                .Include(x => x.JobProposals)
                    .ThenInclude(x => x.HandymanProfile)
                    .ThenInclude(x=>x.User)
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
                .Include(x => x.JobProposals)
                    .ThenInclude(x => x.HandymanProfile)
                    .ThenInclude(x=>x.User)
                .Include(x => x.JobCompletionMarks)
                    .ThenInclude(x => x.MarkedByUser)
                .Include(x => x.JobCancellationMarks)
                    .ThenInclude(x => x.MarkedByUser)
                .Include(x => x.Review);

        }

    }
}