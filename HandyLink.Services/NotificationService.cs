using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Database;
using HandyLink.Services.Exceptions;
using HandyLink.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services
{
    public class NotificationService:INotificationService
    {
        HandyLinkDbContext _dbContext;
        IMapper _mapper;
        
        public NotificationService(HandyLinkDbContext dbContext, IMapper mapper) 
        {
            _dbContext = dbContext;
            _mapper = mapper;
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

    }
}
