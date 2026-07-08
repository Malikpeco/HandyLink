using Azure;
using Azure.Core;
using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Database;
using HandyLink.Services.Database.Entities;
using HandyLink.Services.Exceptions;
using HandyLink.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
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
        private readonly INotificationService _notificationService;

        public ChatService(HandyLinkDbContext dbContext, IMapper mapper, INotificationService notificationService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _notificationService = notificationService;
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

                var creatingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

                if (creatingUser == null)
                    throw new HandyLinkNotFoundException("User not found.");//will never happen

                var receivingUserId =
                job.HandymanProfile.UserId == userId ?
                job.ClientProfile.UserId :
                job.HandymanProfile.UserId;

                await _notificationService.CreateAsync(new NotificationInsertRequest
                {
                    JobId = job.Id,
                    Title = "Chat opened!",
                    Content = $"{creatingUser.FirstName} {creatingUser.LastName} has started a chat with you.",
                    UserId = receivingUserId
                });

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





        public async Task<MessageResponse> SendMessageAsync(int jobId, MessageInsertRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                throw new HandyLinkBusinessRuleException("Message content is required.");
            }

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

            var job = await _dbContext.Jobs.Include(x => x.HandymanProfile).Include(x => x.ClientProfile).FirstOrDefaultAsync(x => x.Id == chat.JobId);

            if (job == null)
                throw new HandyLinkNotFoundException("Job does not exist.");//will never happen.

            if (job.HandymanProfileId == null)
            {
                throw new HandyLinkBusinessRuleException("Cannot send messages before a handyman is assigned.");
            }

            var isClient = job.ClientProfile.UserId == request.SenderUserId;
            var isHandyman = job.HandymanProfile!.UserId == request.SenderUserId;

            if (!isClient && !isHandyman)
            {
                throw new HandyLinkForbiddenException("Only the assigned handyman or client may send messages in this chat.");
            }


            var message = _mapper.Map<Message>(request);

            message.ChatId = chat.Id;
            message.Content = request.Content.Trim();
            message.CreatedAtUtc = DateTime.UtcNow;

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            var sender = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == request.SenderUserId);

            if (sender == null)
            {
                throw new HandyLinkNotFoundException($"User with id {request.SenderUserId} not found.");
            }

            var messageWithSender = await _dbContext.Messages
                .Include(x => x.SenderUser)
                .Include(x => x.Notification)
                .FirstOrDefaultAsync(x => x.Id == message.Id);

            if (messageWithSender == null)
                throw new HandyLinkNotFoundException($"Message not found.");//will never happen, here just for the null reference error.


            var receivingUserId =
               request.SenderUserId == job.ClientProfile.UserId ?
               job.HandymanProfile.UserId :
               job.ClientProfile.UserId;

            await _notificationService.CreateAsync(new NotificationInsertRequest
            {
                JobId = job.Id,
                Title = "New message.",
                Content = $"You have received a message from {sender.FirstName} {sender.LastName}.",
                UserId = receivingUserId
            });


            return _mapper.Map<MessageResponse>(messageWithSender);
        }


    }
}
