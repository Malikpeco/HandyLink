using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class City : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
