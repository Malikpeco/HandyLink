using HandyLink.Model.Responses;
using HandyLink.Services.Database;
using HandyLink.Services.Database.Entities;
using HandyLink.Services.Exceptions;
using HandyLink.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HandyLink.Services
{
    public class ChatService : IChatService
    {
        private HandyLinkDbContext _dbContext;
        private readonly IMapper _mapper;

        public ChatService(HandyLinkDbContext dbContext, IMapper mapper) 
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<ChatResponse> GetOrCreateConversationAsync(int jobId, int userId)
        {
            var job = await _dbContext.Jobs
                .Include(x => x.ClientProfile)
                .Include(x => x.HandymanProfile)
                .FirstOrDefaultAsync(x => x.Id == jobId);
            if (job == null) {
                throw new HandyLinkNotFoundException($"Job with id {jobId} not found.");
            }

            if (job.HandymanProfileId == null)
            {
                throw new HandyLinkBusinessRuleException("Chat is only available after a handyman is assigned.");
            }

            var isClient = job.ClientProfile.UserId == userId;
            var isHandyman = job.HandymanProfile!.UserId == userId;

            if (!isClient && !isHandyman)
            {
                throw new HandyLinkForbiddenException("Only the assigned handyman or client may access this chat.");
            }

            var chat = await _dbContext.Chats
                .Include(x => x.Messages)
                    .ThenInclude(x => x.SenderUser)
                .FirstOrDefaultAsync(x => x.JobId == jobId);

            if (chat == null)
            {
                chat = new Chat
                {
                    JobId = jobId,
                    CreatedAtUtc = DateTime.UtcNow
                };

                _dbContext.Chats.Add(chat);
                await _dbContext.SaveChangesAsync();
            }

            return _mapper.Map<ChatResponse>(chat);

        }

    }
}
