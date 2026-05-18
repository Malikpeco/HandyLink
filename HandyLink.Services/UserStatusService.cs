using FluentValidation;
using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Database;
using HandyLink.Services.Database.Entities;
using HandyLink.Services.Interfaces;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services
{
    public class UserStatusService : BaseCRUDService<UserStatus, UserStatusResponse, UserStatusSearchObject, UserStatusInsertRequest, UserStatusUpdateRequest>, IUserStatusService
    {
        public UserStatusService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<UserStatusInsertRequest> insertValidator, IValidator<UserStatusUpdateRequest> updateValidator) : base(dbContext, mapper, insertValidator, updateValidator)
        {
        }

        protected override IEnumerable<UserStatus> ApplyFilters(IEnumerable<UserStatus> query, UserStatusSearchObject? searchObject)
        {
            if (searchObject?.Name != null)
            {
                var normalized = searchObject.Name.Trim().ToLower();

                query = query.Where(x => x.Name.Trim().ToLower().Contains(normalized));
            }

            return query;
        }
    }
}
