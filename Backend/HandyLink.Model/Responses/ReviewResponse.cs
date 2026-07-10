using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Responses
{
    public class ReviewResponse
    {
        public int Id {  get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public int ClientProfileId { get; set; }
        public string ClientFullName { get; set; } = null!;
        public int HandymanProfileId { get; set; }
        public string HandymanFullName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
