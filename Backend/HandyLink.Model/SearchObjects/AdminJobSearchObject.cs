using HandyLink.Model.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.SearchObjects
{
    public class AdminJobSearchObject : BaseSearchObject
    {
        public string? SearchTerm { get; set; }

        public int? ServiceCategoryId { get; set; }//dropdown
        public int? CityId { get; set; }//dd
        public int? JobStatusId { get; set; }//dd
        public JobCreationType? JobCreationType { get; set; }//dd
        public decimal? MinCurrentPrice { get; set; }//input
        public decimal? MaxCurrentPrice { get; set; }//input
        public DateTime? CreatedFromUtc { get; set; }//input
        public DateTime? CreatedToUtc { get; set; }//input

        public DateTime? ScheduledFromUtc { get; set; }//input
        public DateTime? ScheduledToUtc { get; set; }//input

        public bool? CurrentPriceOnArrangement { get; set; }//checkbox
        public bool? CurrentTimeFlexible { get; set; }//checkbox


    }
}
