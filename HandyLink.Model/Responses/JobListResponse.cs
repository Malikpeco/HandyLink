using HandyLink.Model.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Responses
{
    public class JobListResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ServiceCategoryName { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public string ClientFullName { get; set; } = string.Empty;
        public string? HandymanFullName { get; set; }
        public string JobCreationType { get; set; } = string.Empty;
        public string JobStatusName { get; set; } = string.Empty;
        public string JobStatusCode { get; set; } = string.Empty;
        public decimal? CurrentPrice { get; set; }
        public bool CurrentPriceOnArrangement { get; set; }
        public DateTime CurrentScheduledAtUtc { get; set; }
        public bool CurrentTimeFlexible { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
        public DateTime? CancelledAtUtc { get; set; }
    }
    
}
