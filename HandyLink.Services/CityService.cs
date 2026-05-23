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

namespace HandyLink.Services
{
    public class CityService : BaseCRUDService<City, CityResponse, CitySearchObject, CityInsertRequest, CityUpdateRequest>, ICityService
    {
        public CityService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<CityInsertRequest> insertValidator, IValidator<CityUpdateRequest> updateValidator) : base(dbContext, mapper, insertValidator, updateValidator)
        {
        }


        public override async Task<CityResponse> InsertAsync(CityInsertRequest request)
        {
            await CheckIfCountryExistsAsync(request.CountryId);

            return await base.InsertAsync(request);
        }

        public override async Task<CityResponse> UpdateAsync(int id, CityUpdateRequest request)
        {
            await CheckIfCountryExistsAsync(request.CountryId);

            return await base.UpdateAsync(id, request);
        }

        public override async Task DeleteAsync(int id)
        {
            if(await _dbContext.Users.AnyAsync(x => x.CityId == id) || await _dbContext.Jobs.AnyAsync(x => x.CityId == id))
            {
                throw new HandyLinkBusinessRuleException($"Cannot delete city with id: {id} since it is currently in use.");
            }
            await base.DeleteAsync(id);
            
        }




        private async Task CheckIfCountryExistsAsync(int countryId)
        {
            var exists = await _dbContext.Countries.AnyAsync(c => c.Id == countryId);
            if (!exists)
                throw new HandyLinkNotFoundException("CountryId doesnt exist.");
        }

        protected override IEnumerable<City> ApplyFilters(IEnumerable<City> query, CitySearchObject? searchObject)
        {
            if (searchObject?.Name != null)
            {
                var normalized = searchObject.Name.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(normalized));
            }

            return query;
        }


        

        protected override async Task<IQueryable<City>> IncludeRelatedEntitiesAsync(IQueryable<City> query, CitySearchObject? searchObject)
        {
            return query.Include(c => c.Country);
        }
    }
}
