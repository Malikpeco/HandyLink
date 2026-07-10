using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class Review : BaseEntity
    {
        public int JobId { get; set; }
        public Job Job { get; set; } = null!;
        public int ClientProfileId {  get; set; }
        public ClientProfile ClientProfile { get; set; } = null!;
        public int HandymanProfileId {  get; set; }
        public HandymanProfile HandymanProfile { get; set; } = null!;
        public int Rating {  get; set; }
        public string? Comment {  get; set; }

    }
}
