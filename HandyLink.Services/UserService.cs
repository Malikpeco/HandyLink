using Azure;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services
{
    public class UserService : BaseCRUDService<User, UserResponse, UserSearchObject, UserInsertRequest, UserUpdateRequest>, IUserService
    {
        public UserService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<UserInsertRequest> insertValidator, IValidator<UserUpdateRequest> updateValidator) : base(dbContext, mapper, insertValidator, updateValidator)
        {
        }

        public override async Task<UserResponse> InsertAsync(UserInsertRequest request)
        {
            var validationResult = await _insertValidator.ValidateAsync(request);
            if (validationResult.IsValid == false)
            {
                throw new HandyLinkValidationException(validationResult.Errors);
            }

            var entity = MapInsertRequestToEntity(request);
            var pendingStatus = await _dbContext.UserStatuses.FirstOrDefaultAsync(x => x.Code == "PENDING");
            if (pendingStatus != null)
            {
                entity.UserStatusId = pendingStatus.Id;
            }
            else
            {
                throw new HandyLinkNotFoundException("User status PENDING does not exist.");
            }

                _dbContext.Set<User>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(_mapper.Map<UserResponse>(entity));
        }

        

        protected override IEnumerable<User> ApplyFilters(IEnumerable<User> query, UserSearchObject? searchObject)
        {
            if(searchObject?.SearchTerm != null)
            {
                var normalized = searchObject.SearchTerm.Trim().ToLower();
                query = query
                    .Where(x => ($"{x.FirstName} {x.LastName}").ToLower().Contains(normalized)
                        || x.Email.ToLower().Contains(normalized)
                        || x.PhoneNumber.Contains(normalized));
            }
            if(searchObject?.CityId != null)
            {
                query=query.Where(x=>x.CityId==searchObject.CityId);
            }
            if (searchObject?.UserType != null)
            {
                query=query.Where(x=>x.UserType==searchObject.UserType);
            }
            if (searchObject?.UserStatusId != null)
            {
                query=query.Where(x=>x.UserStatusId==searchObject.UserStatusId);
            }

            return query;
        }

        protected override async Task<IQueryable<User>> IncludeRelatedEntitiesAsync(IQueryable<User> query, UserSearchObject? searchObject)
        {
            query = query.Include(u => u.City);
            query =query.Include(u => u.UserStatus);
            return query;
        }
    }
}
