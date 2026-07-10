using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Requests
{
    public class ReviewInsertRequest
    {
        public int ClientProfileId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
