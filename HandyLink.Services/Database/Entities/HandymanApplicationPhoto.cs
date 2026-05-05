using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class HandymanApplicationPhoto : BaseEntity
    {
        public int HandymanApplicationId { get; set; }
        public HandymanApplication HandymanApplication { get; set; } = null!;
        public string ImageBase64 { get; set; } = string.Empty;
    }
}
