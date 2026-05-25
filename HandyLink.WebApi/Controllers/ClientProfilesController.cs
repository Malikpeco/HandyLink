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
    public class ClientProfilesController : ControllerBase
    {
        private readonly IClientProfileService _service;

        public ClientProfilesController(IClientProfileService service)
        {
            _service = service;
        }

        

        [HttpGet("{id}")]
        public async Task<ClientProfileDetailsResponse> GetById(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        

    }


}
