using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Interfaces
{
    public interface INotificationService
    {
        Task<PageResult<NotificationResponse>> GetMyNotificationsAsync(int userId, NotificationSearchObject? searchObject = null);

        Task<NotificationResponse> CreateAsync(NotificationInsertRequest request);

        Task MarkAsReadAsync(int notificationId, int userId);

    }
}
