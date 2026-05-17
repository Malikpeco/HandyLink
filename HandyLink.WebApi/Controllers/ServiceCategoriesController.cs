using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services;
using HandyLink.Services.Interfaces;

namespace HandyLink.WebApi.Controllers
{
    public class ServiceCategoriesController : BaseCRUDController<ServiceCategoryResponse,ServiceCategorySearchObject,ServiceCategoryInsertRequest,ServiceCategoryUpdateRequest, IServiceCategoryService>
    {
        public ServiceCategoriesController(IServiceCategoryService serviceCategoryService) : base(serviceCategoryService)
        {
        }
    }
}
