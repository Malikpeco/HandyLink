using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandyLink.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewsController(IReviewService service)
        {
            _service = service;
        }

        [HttpPost("job/{jobId}")]
        public async Task<ReviewResponse> Create(int jobId, [FromBody] ReviewInsertRequest request)
        {
            return await _service.CreateReviewAsync(jobId, request);
        }


        [HttpGet("handyman/{handymanProfileId}")]
        public async Task<PageResult<ReviewResponse>> GetHandymanReviews(int handymanProfileId)
        {
            return await _service.GetHandymanReviewsAsync(handymanProfileId);
        }


        [HttpGet("admin")]
        public async Task<PageResult<ReviewResponse>> GetAdminReviews([FromQuery] ReviewSearchObject? search = null)
        {
            return await _service.GetAdminReviewsAsync(search);
        }

    }
}
