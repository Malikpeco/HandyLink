using Azure;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Database;
using HandyLink.Services.Database.Entities;
using HandyLink.Services.Exceptions;
using HandyLink.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

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
        public async Task<ChatResponse> CreateChatAsync(int jobId, int userId)
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
                throw new HandyLinkForbiddenException("Only the assigned handyman or client may create this chat.");
            }

            var existingChat = await _dbContext.Chats.FirstOrDefaultAsync(x => x.JobId == jobId);

            if (existingChat == null)
            {
                var chat = new Chat
                {
                    JobId = jobId,
                    CreatedAtUtc = DateTime.UtcNow
                };

                _dbContext.Chats.Add(chat);
                await _dbContext.SaveChangesAsync();

                return _mapper.Map<ChatResponse>(chat);
            }
            else
            {
                throw new HandyLinkBusinessRuleException($"Chat already exists for job with id {jobId}.");
            }

            
        }





        public virtual async Task<ChatResponse> GetChatAsync(int jobId, int userId, MessageSearchObject? searchObject = null)
        {
            var chat = await _dbContext.Chats
                .Include(x => x.Job)
                    .ThenInclude(x => x.ClientProfile)
                .Include(x => x.Job)
                    .ThenInclude(x => x.HandymanProfile)
                .FirstOrDefaultAsync(x => x.JobId == jobId);

            if (chat == null)
            {
                throw new HandyLinkNotFoundException($"Chat for job with id {jobId} not found.");
            }

            var job = chat.Job;

            var isClient = job.ClientProfile.UserId == userId;
            var isHandyman = job.HandymanProfile != null && job.HandymanProfile.UserId == userId;

            if (!isClient && !isHandyman)
            {
                throw new HandyLinkForbiddenException("Only the assigned handyman or client may view this chat.");
            }

            var messagesQuery = _dbContext.Messages
                .Include(x => x.SenderUser)
                .Where(x => x.ChatId == chat.Id)
                .AsQueryable();

            int? totalCount = null;

            if (searchObject != null)
            {
                if (searchObject.IncludeTotalCount)
                {
                    totalCount = messagesQuery.Count();
                }
                if (!string.IsNullOrWhiteSpace(searchObject.SortBy))
                {
                    messagesQuery = messagesQuery.OrderBy(searchObject.SortBy);
                }
                else
                {
                    messagesQuery = messagesQuery.OrderByDescending(x => x.CreatedAtUtc);
                }

                messagesQuery = messagesQuery.Skip((searchObject.Page - 1) * searchObject.PageSize);
                messagesQuery = messagesQuery.Take(searchObject.PageSize);
            }

            else
            {
                messagesQuery = messagesQuery
                    .OrderByDescending(x => x.CreatedAtUtc)
                     .Take(10);
            }


            var messages = messagesQuery.Select(x => _mapper.Map<MessageResponse>(x)).ToList();

            return new ChatResponse
            {
                Id = chat.Id,
                JobId = chat.JobId,
                CreatedAtUtc = chat.CreatedAtUtc,
                Messages = new PageResult<MessageResponse>
                {
                    Items = messages,
                    TotalCount = totalCount
                }
            };

        }


    }
}
