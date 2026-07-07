using FluentValidation;
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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HandyLink.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HandyLinkDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<NotificationInsertRequest> _insertValidator;


        public NotificationService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<NotificationInsertRequest> insertValidator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _insertValidator = insertValidator;
        }


        public async Task<PageResult<NotificationResponse>> GetMyNotificationsAsync(int userId, NotificationSearchObject? searchObject = null)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                throw new HandyLinkNotFoundException($"User with id {userId} not found.");
            }

            var query = _dbContext.Notifications
                .Where(x => x.UserId == userId)
                .AsQueryable();

            if (searchObject?.IsRead != null)
            {
                query = query.Where(x => x.IsRead == searchObject.IsRead);
            }

            int? totalCount = null;

            if (searchObject != null)
            {
                if (searchObject.IncludeTotalCount)
                {
                    totalCount = query.Count();
                }

                if (!string.IsNullOrWhiteSpace(searchObject.SortBy))
                {
                    query = query.OrderBy(searchObject.SortBy);
                }
                else
                {
                    query = query.OrderByDescending(x => x.CreatedAtUtc);
                }

                query = query.Skip((searchObject.Page - 1) * searchObject.PageSize);
                query = query.Take(searchObject.PageSize);
            }
            else
            {
                query = query.OrderByDescending(x => x.CreatedAtUtc);
            }

            var list = query
                .Select(x => _mapper.Map<NotificationResponse>(x))
                .ToList();

            return await Task.FromResult(new PageResult<NotificationResponse>
            {
                Items = list,
                TotalCount = totalCount
            });
        }









        public async Task<NotificationResponse> CreateAsync(NotificationInsertRequest request)
        {


            var validationResult = await _insertValidator.ValidateAsync(request);
            if (validationResult.IsValid == false)
            {
                throw new HandyLinkValidationException(validationResult.Errors);
            }


            var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==request.UserId);

            if (user == null)
            {
                throw new HandyLinkNotFoundException($"User with id {request.UserId} not found.");
            }

            if (request.JobId.HasValue && request.MessageId.HasValue)
            {
                throw new HandyLinkBusinessRuleException("Notification cannot be linked to both a job and a message.");
            }

            if (request.JobId.HasValue)
            {
                var job = await _dbContext.Jobs.FirstOrDefaultAsync(x => x.Id == request.JobId.Value);

                if (job==null)
                {
                    throw new HandyLinkNotFoundException("Job not found.");
                }
            }

            if (request.MessageId.HasValue)
            {
                var message = await _dbContext.Messages.FirstOrDefaultAsync(x => x.Id == request.MessageId.Value);

                if (message==null)
                {
                    throw new HandyLinkNotFoundException("Message not found.");
                }
            }

            var notification = _mapper.Map<Notification>(request);
            notification.IsRead = false;
            notification.ReadAtUtc = null;
            

            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<NotificationResponse>(notification);

        }



    }
}
