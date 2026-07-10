using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Responses
{
    public class JobCompletionMarkResponse
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public int MarkedByUserId { get; set; }
        public string MarkedByUserFullName { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}
