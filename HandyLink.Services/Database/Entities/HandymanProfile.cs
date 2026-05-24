using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class HandymanProfile : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string Bio { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public ICollection<HandymanServiceCategory> HandymanServiceCategories { get; set; } = new List <HandymanServiceCategory>();
        public ICollection<HandymanWorkPhoto> HandymanWorkPhotos { get; set; } = new List<HandymanWorkPhoto>();
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();


    }
}
