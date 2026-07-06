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
    public interface IChatService
    {
        Task<ChatResponse> CreateChatAsync(int jobId, int userId);
        Task<ChatResponse> GetChatAsync(int jobId, int userId, MessageSearchObject? search = null);
        Task<MessageResponse> SendMessageAsync(int jobId, MessageInsertRequest request);
    }
}
