using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Services;
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

        [HttpPost("accept")]
        public async Task<JobDetailsResponse> AcceptJob(JobAcceptRequest request)
        {
            return await _service.AcceptJobAsync(request);
        }

        [HttpPost("decline")]
        public async Task<JobDetailsResponse> DeclineJob(JobDeclineRequest request)
        {
            return await _service.DeclineJobAsync(request);
        }

        [HttpPost("{id}/suggest-changes")]
        public async Task<JobProposalResponse> SuggestChanges(int id, [FromBody] JobProposalInsertRequest request)
        {
            
            return await _service.SuggestChangesAsync(id, request);
        }

        [HttpPost("completion-mark")]
        public async Task<JobDetailsResponse> MarkAsCompleted([FromBody] JobMarkRequest request)
        {
            return await _service.AddCompletionMarkAsync(request);
        }

        [HttpPost("cancellation-mark")]
        public async Task<JobDetailsResponse> MarkAsCancelled([FromBody] JobMarkRequest request)
        {
            return await _service.AddCancellationMarkAsync(request);
        }


    }
}
