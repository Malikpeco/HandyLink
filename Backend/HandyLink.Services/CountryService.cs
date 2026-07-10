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

        protected override IEnumerable<Country> ApplyFilters(IEnumerable<Country> query, CountrySearchObject? searchObject)
        {
            
            if (searchObject?.Name != null)
            {
                var normalized = searchObject.Name.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(normalized));
            }

            return query;
        }
        


    }
}
