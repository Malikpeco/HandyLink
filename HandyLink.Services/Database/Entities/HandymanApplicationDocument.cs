using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class HandymanApplicationDocument : BaseEntity
    {
        public int HandymanApplicationId { get; set; }
        public HandymanApplication HandymanApplication { get; set; } = null!;
        public string FileUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;

    }
}
