using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandyLink.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatsController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<ActionResult<ChatResponse>> CreateChat(int jobId, [FromQuery] int userId)
        {
            var result = await _chatService.CreateChatAsync(jobId, userId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<ChatResponse>> GetChat(int jobId, [FromQuery] int userId, [FromQuery] MessageSearchObject? searchObject = null)
        {
            var result = await _chatService.GetChatAsync(jobId, userId, searchObject);
            return Ok(result);
        }

    }
}
