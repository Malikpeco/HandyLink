using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandyLink.WebApi.Controllers
{
    public class CitiesController : BaseCRUDController<CityResponse, CitySearchObject, CityInsertRequest, CityUpdateRequest, ICityService>
    {
        public CitiesController(ICityService cityService) : base(cityService)
        {
        }


        [AllowAnonymous]
        [HttpGet]
        public override async Task<PageResult<CityResponse>> GetAll([FromQuery] CitySearchObject? searchObject)
        {
            return await base.GetAll(searchObject);
        }


    }
}