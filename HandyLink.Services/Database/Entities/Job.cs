using HandyLink.Services.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class Job : BaseEntity
    {
        public int ClientProfileId { get; set; }
        public ClientProfile ClientProfile { get; set; } = null!;
        public int? HandymanProfileId { get; set; }
        public HandymanProfile? HandymanProfile { get; set; }
        public int ServiceCategoryId { get; set; }
        public ServiceCategory ServiceCategory { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CityId {  get; set; }
        public City City { get; set; } = null!;
        public string? Address { get; set; }
        public JobCreationType JobCreationType { get; set; }
        public int JobStatusId { get; set; }
        public JobStatus JobStatus { get; set; } = null!;
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
        public ICollection<JobProposal> JobProposals { get; set; } = new List<JobProposal>();

    }
}
