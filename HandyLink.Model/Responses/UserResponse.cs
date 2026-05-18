using HandyLink.Model.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfileImageBase64 { get; set; }
        public string UserType { get; set; } = string.Empty;
        public int CityId { get; set; }
        public string CityName { get; set; } = null!;
        public int UserStatusId { get; set; }
        public string UserStatusName { get; set; } = null!;
    }
}
