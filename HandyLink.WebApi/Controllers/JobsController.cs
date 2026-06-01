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

        [HttpPost("instant-accept-direct-proposal")]
        public async Task<JobDetailsResponse> InstantAcceptDirectProposal(InstantAcceptDirectProposalRequest request)
        {
            return await _service.InstantAcceptDirectProposalAsync(request);
        }

        [HttpPost("instant-decline-direct-proposal")]
        public async Task<JobDetailsResponse> InstantDeclineDirectProposal(InstantDeclineDirectProposalRequest request)
        {
            return await _service.InstantDeclineDirectProposalAsync(request);
        }

        [HttpPost("{jobId}/suggest-changes")]
        public async Task<JobProposalResponse> SuggestChanges(int jobId, [FromBody] JobProposalInsertRequest request)
        {
            
            return await _service.SuggestChangesAsync(jobId, request);
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


        [HttpPost("accept-suggested-changes")]
        public async Task<JobDetailsResponse> AcceptSuggestedChanges([FromBody] JobProposalDecisionRequest request)
        {
            return await _service.AcceptSuggestedChangesAsync(request);
        }


        [HttpPost("decline-suggested-changes")]
        public async Task<JobDetailsResponse> DeclineSuggestedChanges([FromBody] JobProposalDecisionRequest request)
        {
            return await _service.DeclineSuggestedChangesAsync(request);
        }


        [HttpPut("{jobId}/address")]
        public async Task<JobDetailsResponse> AddAddress(int jobId, [FromBody] JobAddressUpdateRequest request)
        {
            return await _service.AddAddressAsync(jobId, request);
        }



    }
}
