using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Interfaces;

namespace HandyLink.WebApi.Controllers
{
    public class UsersController : BaseCRUDController<UserResponse, UserSearchObject, UserInsertRequest, UserUpdateRequest, IUserService>
    {
        public UsersController(IUserService userService) : base(userService)
        {
        }
    }
}
