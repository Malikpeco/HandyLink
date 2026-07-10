using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class HandymanApplicationServiceCategory : BaseEntity
    {
        public int HandymanApplicationId {  get; set; }
        public HandymanApplication HandymanApplication { get; set; } = null!;
        public int ServiceCategoryId { get; set; }
        public ServiceCategory ServiceCategory { get; set; } = null!;
    }
}
