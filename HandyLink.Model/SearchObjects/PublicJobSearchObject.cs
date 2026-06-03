using HandyLink.Model.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.SearchObjects
{
    public class PublicJobSearchObject : BaseSearchObject
    {
        public string? SearchTerm { get; set; }

        public int? ServiceCategoryId { get; set; }//dropdown
        public int? CityId { get; set; }//dd
        public DateTime? ScheduledFromUtc { get; set; }//input
        public DateTime? ScheduledToUtc { get; set; }//input
    }
}
