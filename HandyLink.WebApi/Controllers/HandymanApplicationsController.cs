using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandyLink.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HandymanApplicationsController : ControllerBase
    {
        private readonly IHandymanApplicationService _service;

        public HandymanApplicationsController(IHandymanApplicationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<PageResult<HandymanApplicationListResponse>> GetAll([FromQuery] HandymanApplicationSearchObject? search = null)
        {
            return await _service.GetAllAsync(search);
        }

        [HttpGet("{id}")]
        public async Task<HandymanApplicationDetailsResponse> GetById(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<HandymanApplicationDetailsResponse> Create([FromBody] HandymanApplicationInsertRequest request)
        {
            return await _service.InsertAsync(request);
        }



        [HttpPut("{id}/decision")]
        public async Task<HandymanApplicationDetailsResponse> SetDecision(int id, [FromBody]HandymanApplicationDecisionRequest request)
        {
            return await _service.SetDecisionAsync(id, request);
        }
    }


}
