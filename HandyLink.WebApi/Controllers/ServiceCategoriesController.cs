using Azure;
using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services;
using HandyLink.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandyLink.WebApi.Controllers
{
    public class ServiceCategoriesController : BaseCRUDController<ServiceCategoryResponse,ServiceCategorySearchObject,ServiceCategoryInsertRequest,ServiceCategoryUpdateRequest, IServiceCategoryService>
    {
        public ServiceCategoriesController(IServiceCategoryService serviceCategoryService) : base(serviceCategoryService)
        {

        }


        [AllowAnonymous]
        [HttpGet]
        public override async Task<PageResult<ServiceCategoryResponse>> GetAll([FromQuery] ServiceCategorySearchObject? searchObject)
        {
            return await base.GetAll(searchObject);
        }
    }
}
