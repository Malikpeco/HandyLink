using FluentValidation;
using HandyLink.Model.Database.Enums;
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



        public async Task<JobResponse> CreateJobAsync(JobInsertRequest request)
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
            entity.CurrentPriceOnArrangement=request.InitialPriceOnArrangement;

            if (request.InitialScheduledAtUtc == default)
                throw new HandyLinkBusinessRuleException("InitalScheduledAt is required.");

            entity.CurrentTimeFlexible = request.InitialTimeFlexible;

            entity.InitialScheduledAtUtc = request.InitialTimeFlexible 
                ? request.InitialScheduledAtUtc.Date 
                : request.InitialScheduledAtUtc;

            entity.CurrentScheduledAtUtc = entity.InitialScheduledAtUtc;

            _dbContext.Jobs.Add(entity);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<JobResponse>(entity);
        }



    }
}
