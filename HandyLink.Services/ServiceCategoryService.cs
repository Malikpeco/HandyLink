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
    public class ServiceCategoryService : BaseCRUDService<ServiceCategory, ServiceCategoryResponse, ServiceCategorySearchObject, ServiceCategoryInsertRequest, ServiceCategoryUpdateRequest>, IServiceCategoryService
    {
        public ServiceCategoryService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<ServiceCategoryInsertRequest> insertValidator, IValidator<ServiceCategoryUpdateRequest> updateValidator) : base(dbContext, mapper, insertValidator, updateValidator)
        {
        }

        protected override IEnumerable<ServiceCategory> ApplyFilters(IEnumerable<ServiceCategory> query, ServiceCategorySearchObject? searchObject)
        {
            if (searchObject?.Name != null)
            {
                query = query.Where(sc => sc.Name.ToLower().Contains(searchObject.Name.ToLower()));
            }
            if (searchObject?.IsActive != null)
            {
                query = query.Where(sc => sc.IsActive == searchObject.IsActive);
            }

            return query;
        }
    }
}
