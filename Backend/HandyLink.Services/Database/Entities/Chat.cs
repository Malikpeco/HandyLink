using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class Chat : BaseEntity
    {
        public int JobId { get; set; }
        public Job Job { get; set; } = null!;
        public ICollection<Message> Messages { get; set; } = new List<Message>();

    }
}
