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
    public class CityService : BaseCRUDService<City, CityResponse, CitySearchObject, CityInsertRequest, CityUpdateRequest>, ICityService
    {
        public CityService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<CityInsertRequest> insertValidator, IValidator<CityUpdateRequest> updateValidator) : base(dbContext, mapper, insertValidator, updateValidator)
        {
        }
        public override async Task<CityResponse> InsertAsync(CityInsertRequest request)
        {
            await CheckCityNameDuplicateAsync(request.Name, request.CountryId);

            return await base.InsertAsync(request);
        }


        public override async Task<CityResponse> UpdateAsync(int id, CityUpdateRequest request)
        {
            await CheckCityNameDuplicateAsync(request.Name, request.CountryId, id);

            return await base.UpdateAsync(id, request);
        }

        protected override IEnumerable<City> ApplyFilters(IEnumerable<City> query, CitySearchObject? searchObject)
        {
            if (searchObject?.Name != null)
            {
                query = query.Where(c => c.Name.ToLower().Contains(searchObject.Name.ToLower()));
            }

            return query;
        }
        private async Task CheckCityNameDuplicateAsync(string? name, int? countryId, int? id = null)
        {
            var normalizedName = name?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(normalizedName) || countryId is null)
                return;

            if (await _dbContext.Cities.AnyAsync(c => (c.Name.Trim().ToLower() == normalizedName && c.CountryId==countryId) && c.Id != id))
                throw new HandyLinkConflictException($"City with name {name?.Trim()} already exists in this country.");
        }

        protected override async Task<IQueryable<City>> IncludeRelatedEntitiesAsync(IQueryable<City> query, CitySearchObject? searchObject)
        {
            query = query.Include(c => c.Country);
            return query;
        }
    }
}
