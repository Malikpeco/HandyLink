using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class Message : BaseEntity
    {
        public int ChatId { get; set; }
        public Chat Chat { get; set; } = null!;
        public int SenderUserId { get; set; }
        public User SenderUser { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public Notification? Notification { get; set; }
    }
}
