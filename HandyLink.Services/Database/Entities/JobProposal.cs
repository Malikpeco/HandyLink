using HandyLink.Model.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class JobProposal : BaseEntity
    {
        public int JobId { get; set; }
        public Job Job { get; set; } = null!;
        public int ProposedByUserId { get; set; }
        public User ProposedByUser { get; set; } = null!;
        public decimal? ProposedPrice { get; set; }
        public bool ProposedPriceOnArrangement { get; set; }
        public DateTime ProposedScheduledAtUtc { get; set; }
        public bool ProposedTimeFlexible { get; set; }
        public JobProposalStatus JobProposalStatus { get; set; }
    }
}
