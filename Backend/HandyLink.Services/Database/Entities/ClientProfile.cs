using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class ClientProfile : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        
    }
}
