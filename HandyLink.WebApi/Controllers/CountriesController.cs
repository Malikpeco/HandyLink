using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services;
using HandyLink.Services.Interfaces;

namespace HandyLink.WebApi.Controllers
{
    public class CountriesController : BaseCRUDController<CountryResponse,CountrySearchObject,CountryInsertRequest,CountryUpdateRequest, ICountryService>
    {
        public CountriesController(ICountryService countryService) : base(countryService)
        {
        }
    }
}
