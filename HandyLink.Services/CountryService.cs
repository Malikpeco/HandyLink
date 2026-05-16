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
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services
{
    public class CountryService : BaseCRUDService<Country, CountryResponse, CountrySearchObject, CountryInsertRequest, CountryUpdateRequest>, ICountryService
    {
        public CountryService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<CountryInsertRequest> insertValidator, IValidator<CountryUpdateRequest> updateValidator) : base(dbContext, mapper, insertValidator, updateValidator)
        {
        }

        public override async Task<CountryResponse> InsertAsync(CountryInsertRequest request)
        {
            await CheckCountryNameDuplicateAsync(request.Name);

            return await base.InsertAsync(request);
        }


        public override async Task<CountryResponse> UpdateAsync(int id, CountryUpdateRequest request)
        {
            await CheckCountryNameDuplicateAsync(request.Name, id);

            return await base.UpdateAsync(id, request);
        }


        protected override IEnumerable<Country> ApplyFilters(IEnumerable<Country> query, CountrySearchObject? searchObject)
        {
            
            if (searchObject?.Name != null)
            {
                query = query.Where(c => c.Name.ToLower().Contains(searchObject.Name.ToLower()));
            }

            return query;
        }
        private async Task CheckCountryNameDuplicateAsync(string? name, int? id = null)
        {
            var normalizedName = name?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(normalizedName))
                return;

            if (await _dbContext.Countries.AnyAsync(c => c.Name.Trim().ToLower() == normalizedName && c.Id != id))
                throw new HandyLinkConflictException($"Country with name {name?.Trim()} already exists.");
        }


    }
}
