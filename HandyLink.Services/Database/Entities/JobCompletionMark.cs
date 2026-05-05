using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class JobCompletionMark : BaseEntity
    {
        public int JobId { get; set; }
        public Job Job { get; set; } = null!;
        public int MarkedByUserId  { get; set; }
        public User MarkedByUser { get; set; } = null!;
        public DateTime MarkedAtUtc { get; set; }
    }
}
