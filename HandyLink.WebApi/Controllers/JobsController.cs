using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandyLink.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _service;

        public JobsController(IJobService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<JobDetailsResponse> Create([FromBody] JobInsertRequest request)
        {
            return await _service.CreateJobAsync(request);
        }


        [HttpGet("{id}")]
        public async Task<JobDetailsResponse> GetById(int id)
        {
            return await _service.GetByIdAsync(id);
        }
    }
}
