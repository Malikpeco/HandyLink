using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Interfaces;

namespace HandyLink.WebApi.Controllers
{
    public class CitiesController : BaseCRUDController<CityResponse, CitySearchObject, CityInsertRequest, CityUpdateRequest, ICityService>
    {
        public CitiesController(ICityService cityService) : base(cityService)
        {
        }
    }
}