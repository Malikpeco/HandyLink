using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Responses
{
    public class ClientProfileResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserResponse User { get; set; } = null!;
        public int CompletedJobs { get; set; }
        public int ReviewsCount { get; set; }
    }

}
