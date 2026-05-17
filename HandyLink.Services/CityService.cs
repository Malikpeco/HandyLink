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
                query = query.Where(c => c.Name.ToLower().Contains(searchObject.Name.ToLower()));
            }

            return query;
        }


        

        protected override async Task<IQueryable<City>> IncludeRelatedEntitiesAsync(IQueryable<City> query, CitySearchObject? searchObject)
        {
            query = query.Include(c => c.Country);
            return query;
        }
    }
}
