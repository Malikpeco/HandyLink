using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class HandymanWorkPhoto : BaseEntity
    {
        public int HandymanProfileId {  get; set; }
        public HandymanProfile HandymanProfile { get; set; } = null!;
        public string ImageBase64 { get; set; } = string.Empty;
    }
}
