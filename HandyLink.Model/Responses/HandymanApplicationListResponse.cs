using HandyLink.Model.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Responses
{
    public class HandymanApplicationListResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserFirstName { get; set; } = string.Empty;
        public string UserLastName { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }

    }
}
