using Azure;
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
using System.Linq.Expressions;


namespace HandyLink.Services
{
    public class HandymanApplicationService : IHandymanApplicationService
    {
        private readonly HandyLinkDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<HandymanApplicationInsertRequest> _insertValidator;

        public HandymanApplicationService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<HandymanApplicationInsertRequest> insertValidator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _insertValidator = insertValidator;
        }



        public async Task<HandymanApplicationDetailsResponse> InsertAsync(HandymanApplicationInsertRequest request)
        {
            var validationResult = await _insertValidator.ValidateAsync(request);
            if (validationResult.IsValid == false)
            {
                throw new HandyLinkValidationException(validationResult.Errors);
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==request.UserId);
            if (user == null)
                throw new HandyLinkNotFoundException($"User with id {request.UserId} not found");

            if (user.UserType != UserType.Handyman)
                throw new HandyLinkNotFoundException($"User must have UserType.Handyman.");

            var serviceCategoryIdsExist = true;
            foreach (int id in request.ServiceCategoryIds)
            {
                serviceCategoryIdsExist = await _dbContext.ServiceCategories.AnyAsync(x => x.Id == id);
                if (!serviceCategoryIdsExist)
                {
                    throw new HandyLinkNotFoundException($"ServiceCategoryId {id} does not exist.");
                }
            }

            var entity = _mapper.Map<HandymanApplication>(request);

            entity.Status = HandymanApplicationStatus.Pending;

            _dbContext.HandymanApplications.Add(entity);

            await _dbContext.SaveChangesAsync();
            return await Task.FromResult(_mapper.Map<HandymanApplicationDetailsResponse>(entity));
        }



        public async Task<HandymanApplicationDetailsResponse> GetByIdAsync(int id)
        {
            var query = _dbContext.HandymanApplications.AsQueryable();
            query = IncludeRelatedEntities(query, null);

            var entity = await query.FirstOrDefaultAsync(e => e.Id == id);

            if (entity == null)
            {
                throw new HandyLinkNotFoundException($"HandymanApplication with id {id} not found.");
            }

            return await Task.FromResult(_mapper.Map<HandymanApplicationDetailsResponse>(entity));
        }



        public async Task<PageResult<HandymanApplicationListResponse>> GetAllAsync(
            HandymanApplicationSearchObject? searchObject = null)
        {
            var query = _dbContext.HandymanApplications.AsQueryable();

            query = IncludeRelatedEntities(query, null);

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

            var list = query.Select(item => _mapper.Map<HandymanApplicationListResponse>(item)).ToList();

            var pageResult = new PageResult<HandymanApplicationListResponse>
            {
                Items = list,
                TotalCount = totalCount,
            };

            return await Task.FromResult(pageResult);
        }




        public async Task DeleteAsync(int id)
        {
            var entity = await _dbContext.HandymanApplications.FirstOrDefaultAsync(x=>x.Id==id);

            if (entity == null)
            {
                throw new HandyLinkNotFoundException($"HandymanApplication with id {id} does not exist.");
            }

            _dbContext.HandymanApplications.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }


        private IQueryable<HandymanApplication> ApplyFilters(IQueryable<HandymanApplication> query, HandymanApplicationSearchObject? searchObject)
        {
            if(searchObject?.SearchTerm != null)
            {
                var normalized = searchObject.SearchTerm.Trim().ToLower();
                query = query
                    .Where(x => ($"{x.User.FirstName} {x.User.LastName}").ToLower().Contains(normalized));
            }
            if(searchObject?.Status != null)
            {
                query = query.Where(x => x.Status == searchObject.Status);
            }
            return query;
        }


        private IQueryable<HandymanApplication> IncludeRelatedEntities(IQueryable<HandymanApplication> query, HandymanApplicationSearchObject? searchObject)
        {

            return query
                .Include(x => x.User)
                    .ThenInclude(x => x.City)
                .Include(x => x.HandymanApplicationServiceCategories)
                    .ThenInclude(x => x.ServiceCategory)

                .Include(x => x.HandymanApplicationPhotos)

                .Include(x => x.HandymanApplicationDocuments)

                .Include(x => x.HandymanApplicationReferences)

                .Include(x => x.User).ThenInclude(x => x.UserStatus);
                
        }

        public async Task<HandymanApplicationDetailsResponse> SetDecisionAsync(int id, HandymanApplicationDecisionRequest request)
        {
            var application = await _dbContext.HandymanApplications.FirstOrDefaultAsync(x => x.Id == id);
            if (application == null) {
                throw new HandyLinkNotFoundException($"HandymanApplication with id {id} not found.");
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == application.UserId);
            if (user == null)
                throw new HandyLinkNotFoundException($"HandymanApplication is not assigned to a valid user. User with id {application.UserId} does not exist.");

            if (application.Status != HandymanApplicationStatus.Pending)
            {
                throw new HandyLinkBusinessRuleException("Only pending applications can be approved or rejected.");
            }

            if (user.UserType != UserType.Handyman)
            {
                throw new HandyLinkBusinessRuleException("User must have UserType.Handyman.");
            }


            var activeStatus = await _dbContext.UserStatuses.FirstOrDefaultAsync(x => x.Code == "ACTIVE");
            if (activeStatus == null)
            {
                throw new HandyLinkNotFoundException("User status ACTIVE does not exist.");
            }
            var rejectedStatus = await _dbContext.UserStatuses.FirstOrDefaultAsync(x => x.Code == "REJECTED");
            if (rejectedStatus == null)
            {
                throw new HandyLinkNotFoundException("User status REJECTED does not exist.");
            }



            if (request.IsApproved)
            {
                application.Status = HandymanApplicationStatus.Approved;
                user.UserStatusId = activeStatus.Id;
            }
            else
            {
                application.Status = HandymanApplicationStatus.Rejected;
                user.UserStatusId = rejectedStatus.Id;
            }


            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(_mapper.Map<HandymanApplicationDetailsResponse>(application));

        }
    }
}
