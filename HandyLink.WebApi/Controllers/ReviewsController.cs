using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
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

        [HttpPost]
        public async Task<ReviewResponse> Create(int jobId, [FromBody] ReviewInsertRequest request)
        {
            return await _service.CreateReviewAsync(jobId, request);
        }

    }
}
