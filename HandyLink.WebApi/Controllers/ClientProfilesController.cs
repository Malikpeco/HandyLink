using Azure;
using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandyLink.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientProfilesController : BaseReadController<ClientProfileResponse, ClientProfileSearchObject, IClientProfileService>
    {

        public ClientProfilesController(IClientProfileService service) : base(service)
        {

        }
        
    




    }


}
