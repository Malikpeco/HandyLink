using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HandyLink.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet("my/{userId}")]
        public async Task<PageResult<NotificationResponse>> GetMyNotifications(int userId, [FromQuery] NotificationSearchObject? search = null)
        {
            return await _service.GetMyNotificationsAsync(userId, search);
        }


        [HttpPut("{notificationId}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(int notificationId, int userId)
        {
            await _service.MarkAsReadAsync(notificationId, userId);
            return NoContent();
        }

    }
}
