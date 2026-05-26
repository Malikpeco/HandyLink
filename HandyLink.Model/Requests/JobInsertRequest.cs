using HandyLink.Model.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Requests
{
    public class JobInsertRequest
    {
        public int ClientProfileId { get; set; }//remove when currentuserservice is made and make it automatic take the client thats logged in and make sure its client profile.
        public int? HandymanProfileId { get; set; }
        public int ServiceCategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CityId { get; set; }
        public string? Address { get; set; }
        public JobCreationType JobCreationType { get; set; }
        public decimal? InitialPrice { get; set; }
        public bool InitialPriceOnArrangement { get; set; }
        public DateTime InitialScheduledAtUtc { get; set; }
        public bool InitialTimeFlexible { get; set; }


    }
}
