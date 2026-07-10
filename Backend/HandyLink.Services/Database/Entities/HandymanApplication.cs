using HandyLink.Model.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class HandymanApplication : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int ExperienceYears { get; set; }
        public string WorkDescription { get; set; } = string.Empty;
        public HandymanApplicationStatus Status { get; set; }
        public ICollection<HandymanApplicationServiceCategory> HandymanApplicationServiceCategories { get; set; } = new List<HandymanApplicationServiceCategory>();
        public ICollection<HandymanApplicationPhoto> HandymanApplicationPhotos { get; set; } = new List<HandymanApplicationPhoto>();
        public ICollection<HandymanApplicationDocument> HandymanApplicationDocuments { get; set; } = new List<HandymanApplicationDocument>();
        public ICollection<HandymanApplicationReference> HandymanApplicationReferences { get; set; } = new List<HandymanApplicationReference>();

    }
}
