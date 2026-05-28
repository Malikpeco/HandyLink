using HandyLink.Model.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Responses
{
    public class JobDetailsResponse
    {
        public int Id { get; set; }
        public int ClientProfileId { get; set; }
        public string ClientFullName { get; set; } = string.Empty;
        public int? HandymanProfileId { get; set; }
        public string? HandymanFullName { get; set; }
        public int ServiceCategoryId { get; set; }
        public string ServiceCategoryName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string JobCreationType { get; set; } = string.Empty;
        public int JobStatusId { get; set; }
        public string JobStatusName { get; set; } = string.Empty;
        public decimal? InitialPrice { get; set; }
        public decimal? CurrentPrice { get; set; }
        public bool InitialPriceOnArrangement { get; set; }
        public bool CurrentPriceOnArrangement { get; set; }
        public DateTime InitialScheduledAtUtc { get; set; }
        public DateTime CurrentScheduledAtUtc { get; set; }
        public bool InitialTimeFlexible { get; set; }
        public bool CurrentTimeFlexible { get; set; }
        public DateTime? ConfirmedAtUtc { get; set; }
        public DateTime? CancelledAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public List<JobProposalResponse> JobProposals { get; set; } = new();
        public List<JobCompletionMarkResponse> JobCompletionMarks { get; set; } = new();
        public List<JobCancellationMarkResponse> JobCancellationMarks { get; set; } = new();
        public ReviewResponse? Review { get; set; }
    }
}
