using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandyLink.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HandymanProfilesController : ControllerBase
    {
        private readonly IHandymanProfileService _service;

        public HandymanProfilesController(IHandymanProfileService service)
        {
            _service = service;
        }

        //[HttpGet]
        //public async Task<PageResult<HandymanProfileListResponse>> GetAll([FromQuery] HandymanProfileSearchObject? search = null)
        //{
        //    return await _service.GetAllAsync(search);
        //}

        [HttpGet("{id}")]
        public async Task<HandymanProfileDetailsResponse> GetById(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<HandymanProfileDetailsResponse> Create([FromBody] HandymanProfileInsertRequest request)
        {
            return await _service.InsertAsync(request);
        }

        //[HttpDelete("{id}")]
        //public async Task Delete(int id)
        //{
        //    await _service.DeleteAsync(id);
        //}

    }


}
