using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Responses
{
    public class MessageResponse
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int SenderUserId { get; set; }
        public string SenderFullName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}
