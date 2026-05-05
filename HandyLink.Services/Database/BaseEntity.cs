using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime ModifiedAtUtc { get; set; }
        public int? CreatedById { get; set; }
        public int? ModifiedById { get; set; }
        public bool IsDeleted { get; set; }

    }
}
