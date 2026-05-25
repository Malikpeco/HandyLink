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
    public class ClientProfileService : IClientProfileService
    {
        private readonly HandyLinkDbContext _dbContext;
        private readonly IMapper _mapper;
        //private readonly IValidator<ClientProfileInsertRequest> _insertValidator;
        //private readonly IValidator<ClientProfileUpdateRequest> _updateValidator;
        
        public ClientProfileService(HandyLinkDbContext dbContext, IMapper mapper/*, IValidator<ClientProfileInsertRequest> insertValidator, IValidator<ClientProfileUpdateRequest> updateValidator*/)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            //_insertValidator = insertValidator;
            //_updateValidator = updateValidator;
        }

        

        public async Task<ClientProfileDetailsResponse> GetByIdAsync(int id)
        {
            var query = _dbContext.ClientProfiles.AsQueryable();
            query = IncludeRelatedEntities(query, null);

            var profile = await query.FirstOrDefaultAsync(x => x.Id == id);

            if (profile == null)
                throw new HandyLinkNotFoundException($"ClientProfile with id {id} not found.");
            
            var response = _mapper.Map<ClientProfileDetailsResponse>(profile);

            return response;
        }


        






        private IEnumerable<ClientProfile> ApplyFilters(IEnumerable<ClientProfile> query, ClientProfileSearchObject? searchObject)
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
            
            return query;
        }

        private IQueryable<ClientProfile> IncludeRelatedEntities(IQueryable<ClientProfile> query, ClientProfileSearchObject? searchObject)
        {

            return query
                .Include(x => x.User)
                    .ThenInclude(x => x.City)

                .Include(x => x.User).ThenInclude(x => x.UserStatus);
            

        }

    }
}
