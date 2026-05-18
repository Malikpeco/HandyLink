using HandyLink.Model.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.SearchObjects
{
    public class UserSearchObject : BaseSearchObject
    {
        public string? SearchTerm { get; set; }
        public int? CityId { get; set; }
        public UserType? UserType { get; set; }
        public int? UserStatusId { get; set; }
    }
}
