using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class ServiceCategory : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public ICollection<HandymanServiceCategory> HandymanServiceCategories { get; set; } = new List<HandymanServiceCategory>();
        public ICollection<HandymanApplicationServiceCategory> HandymanApplicationServiceCategories { get; set; } = new List<HandymanApplicationServiceCategory>();
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
