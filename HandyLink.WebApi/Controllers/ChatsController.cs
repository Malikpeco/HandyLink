using HandyLink.Model.Responses;
using HandyLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandyLink.WebApi.Controllers
{
    [ApiController]
    [Route("chat")]
    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatsController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        public async Task<ActionResult<ChatResponse>> GetOrCreateConversation(int jobId, [FromQuery] int userId)
        {
            var result = await _chatService.GetOrCreateConversationAsync(jobId, userId);
            return Ok(result);
        }
    }
}
