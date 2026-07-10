using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Interfaces;

namespace HandyLink.WebApi.Controllers
{
    public class UserStatusesController : BaseCRUDController<UserStatusResponse, UserStatusSearchObject, UserStatusInsertRequest, UserStatusUpdateRequest, IUserStatusService>
    {
        public UserStatusesController(IUserStatusService userStatusService) : base(userStatusService)
        {
        }
    }
}
