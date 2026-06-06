using Azure;
using FluentValidation;
using HandyLink.Model.Database.Enums;
using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Database;
using HandyLink.Services.Database.Entities;
using HandyLink.Services.Exceptions;
using HandyLink.Services.Hashing;
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

        private readonly IHashingService _hashingService;

        public UserService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<UserInsertRequest> insertValidator, IValidator<UserUpdateRequest> updateValidator, IHashingService hashingService) : base(dbContext, mapper, insertValidator, updateValidator)
        {
            _hashingService = hashingService;
        }

        public override async Task<UserResponse> InsertAsync(UserInsertRequest request)
        {
            var validationResult = await _insertValidator.ValidateAsync(request);
            if (validationResult.IsValid == false)
            {
                throw new HandyLinkValidationException(validationResult.Errors);
            }

            var emailExists = await _dbContext.Users.AnyAsync(x => x.Email == request.Email && x.UserStatus.Code == "ACTIVE");
            var phoneNumberExists = await _dbContext.Users.AnyAsync(x => x.PhoneNumber==request.PhoneNumber && x.UserStatus.Code == "ACTIVE");
            if (emailExists)
                throw new HandyLinkValidationException("User with this email already exists.");
            if (phoneNumberExists)
                throw new HandyLinkValidationException("User with this phone number already exists.");


            var entity = MapInsertRequestToEntity(request);
            
            var pendingStatus = await _dbContext.UserStatuses.FirstOrDefaultAsync(x => x.Code == "PENDING");
            if (pendingStatus == null)
            {
                throw new HandyLinkNotFoundException("User status PENDING does not exist.");
            }

            var activeStatus = await _dbContext.UserStatuses.FirstOrDefaultAsync(x => x.Code == "ACTIVE");
            if (activeStatus == null)
            {
                throw new HandyLinkNotFoundException("User status ACTIVE does not exist.");
            }

            var salt = _hashingService.GenerateSalt();
            entity.PasswordSalt = salt;
            entity.PasswordHash = _hashingService.HashText(request.Password, salt);

            _dbContext.Users.Add(entity);

            if(request.UserType == UserType.Admin)
            {
                _dbContext.AdminProfiles.Add(new AdminProfile
                {
                    User = entity
                });
                entity.UserStatusId = activeStatus.Id;
            }
            else if (request.UserType == UserType.Client)
            {
                _dbContext.ClientProfiles.Add(new ClientProfile
                {
                    User = entity
                });
                entity.UserStatusId = activeStatus.Id;
            }
            else if (request.UserType == UserType.Handyman)
            {
                entity.UserStatusId = pendingStatus.Id;
            }


                await _dbContext.SaveChangesAsync();

            return await Task.FromResult(_mapper.Map<UserResponse>(entity));
        }


        public override async Task DeleteAsync(int id)
        {
            var entity = await _dbContext.Set<User>().Include(x=>x.HandymanProfile).Include(x=>x.ClientProfile).FirstOrDefaultAsync(x=>x.Id==id);

            if (entity == null)
                throw new HandyLinkNotFoundException($"User with id {id} not found.");


            if (entity.HandymanProfile != null)
            {
                _dbContext.HandymanProfiles.Remove(entity.HandymanProfile);
            }
            if (entity.ClientProfile != null)
            {
                _dbContext.ClientProfiles.Remove(entity.ClientProfile);
            }

            _dbContext.Set<User>().Remove(entity);
            await _dbContext.SaveChangesAsync();
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
