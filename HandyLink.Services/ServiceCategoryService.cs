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
    public class ServiceCategoryService : BaseCRUDService<ServiceCategory, ServiceCategoryResponse, ServiceCategorySearchObject, ServiceCategoryInsertRequest, ServiceCategoryUpdateRequest>, IServiceCategoryService
    {
        public ServiceCategoryService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<ServiceCategoryInsertRequest> insertValidator, IValidator<ServiceCategoryUpdateRequest> updateValidator) : base(dbContext, mapper, insertValidator, updateValidator)
        {
        }

        public override async Task DeleteAsync(int id)
        {
            if (await _dbContext.HandymanServiceCategories.AnyAsync(x => x.ServiceCategoryId == id)
                || await _dbContext.HandymanApplicationServiceCategories.AnyAsync(x => x.ServiceCategoryId == id)
                || await _dbContext.Jobs.AnyAsync(x => x.ServiceCategoryId == id))
                throw new HandyLinkBusinessRuleException($"ServiceCategory with id {id} is being used and cannot be deleted.");

            await base.DeleteAsync(id);
        }

        protected override IEnumerable<ServiceCategory> ApplyFilters(IEnumerable<ServiceCategory> query, ServiceCategorySearchObject? searchObject)
        {
            if (searchObject?.Name != null)
            {
                var normalized = searchObject.Name.Trim().ToLower();
                query = query.Where(sc => sc.Name.ToLower().Contains(normalized));
            }
            if (searchObject?.IsActive != null)
            {
                query = query.Where(sc => sc.IsActive == searchObject.IsActive);
            }

            return query;
        }
    }
}
