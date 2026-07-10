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
    public class ClientProfileService : BaseReadService<ClientProfile, ClientProfileResponse, ClientProfileSearchObject>, IClientProfileService
    {
        public ClientProfileService(IMapper mapper, HandyLinkDbContext dbContext) : base(mapper, dbContext)
        {
        }

        public override async Task<ClientProfileResponse> GetByIdAsync(int id)
        {
            var query = _dbContext.ClientProfiles.AsQueryable();
            query = await IncludeRelatedEntitiesAsync(query, null);

            var profile = await query.FirstOrDefaultAsync(x => x.Id == id);

            if (profile == null)
                throw new HandyLinkNotFoundException($"ClientProfile with id {id} not found.");
            
            var response = _mapper.Map<ClientProfileResponse>(profile);

            return response;
        }


        public override async Task<PageResult<ClientProfileResponse>> GetAllAsync(ClientProfileSearchObject? searchObject = null)
        {
            IEnumerable<ClientProfile> query = _dbContext.Set<ClientProfile>();

            query = await IncludeRelatedEntitiesAsync(query.AsQueryable(), searchObject);
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

            var list = query.Select(item => _mapper.Map<ClientProfileResponse>(item)).ToList();

            var pageResult = new PageResult<ClientProfileResponse>
            {
                Items = list,
                TotalCount = totalCount,
            };

            return await Task.FromResult(pageResult);
        }









        protected override IEnumerable<ClientProfile> ApplyFilters(IEnumerable<ClientProfile> query, ClientProfileSearchObject? searchObject)
        {
            if (searchObject?.SearchTerm != null)
            {
                var normalized = searchObject.SearchTerm.Trim().ToLower();
                query = query
                    .Where(x => (x.User.FirstName + " " + x.User.LastName).ToLower().Contains(normalized));
            }
            if (searchObject?.CityId != null)
            {
                query = query.Where(x => x.User.CityId == searchObject.CityId);
            }
            if (searchObject?.UserStatus != null)
            {
                query = query.Where(x => x.User.UserStatus.Code == searchObject.UserStatus);
            }
            return query;
        }





        protected override async Task<IQueryable<ClientProfile>> IncludeRelatedEntitiesAsync(IQueryable<ClientProfile> query, ClientProfileSearchObject? searchObject)
        {

            return query
                .Include(x => x.User).ThenInclude(x => x.City)
                .Include(x=>x.Reviews)
                .Include(x=>x.Jobs).ThenInclude(x => x.JobStatus)
                .Include(x => x.User).ThenInclude(x => x.UserStatus);

            

        }

        
    }
}
