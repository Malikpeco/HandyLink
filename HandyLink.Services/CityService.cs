using FluentValidation;
using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Database;
using HandyLink.Services.Database.Entities;
using HandyLink.Services.Interfaces;
using MapsterMapper;
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

        protected override IEnumerable<City> ApplyFilters(IEnumerable<City> query, CitySearchObject? searchObject)
        {
            if (searchObject?.Name != null)
            {
                query = query.Where(c => c.Name.ToLower().Contains(searchObject.Name.ToLower()));
            }

            return query;
        }
    }
}
