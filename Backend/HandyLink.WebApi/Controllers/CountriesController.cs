using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services;
using HandyLink.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandyLink.WebApi.Controllers
{
    public class CountriesController : BaseCRUDController<CountryResponse,CountrySearchObject,CountryInsertRequest,CountryUpdateRequest, ICountryService>
    {
        public CountriesController(ICountryService countryService) : base(countryService)
        {
        }



        [AllowAnonymous]
        [HttpGet]
        public override async Task<PageResult<CountryResponse>> GetAll([FromQuery] CountrySearchObject? searchObject)
        {
            return await base.GetAll(searchObject);
        }

    }
}
