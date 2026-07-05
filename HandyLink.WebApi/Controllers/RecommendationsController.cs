using HandyLink.Model.Responses;
using HandyLink.Services;
using HandyLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandyLink.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecommendationsController : ControllerBase
    {
        private readonly IHandymanRecommendationService _recommendationService;

        public RecommendationsController(IHandymanRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet("{clientProfileId}")]
        public async Task<IReadOnlyList<RecommendedHandymanProfileResponse>> GetRecommendedHandymen(
            int clientProfileId,
            [FromQuery] int take = 3,
            CancellationToken cancellationToken = default)
        {
            return await _recommendationService.RecommendAsync(
                clientProfileId,
                take,
                cancellationToken);
        }
    }
}
