using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Interfaces
{
    public interface IChatService
    {
        Task<ChatResponse> GetOrCreateConversationAsync(int jobId, int userId);
        //Task<List<MessageResponse>> GetMessagesAsync(int jobId, int userId);
        //Task<MessageResponse> SendMessageAsync(int jobId, MessageInsertRequest request);
    }
}
