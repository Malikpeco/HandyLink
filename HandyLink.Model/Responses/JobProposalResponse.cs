using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Responses
{
    public class JobProposalResponse
    {
        public int Id { get; set; } 
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public int ProposedByUserId { get; set; }
        public string ProposedByUserFullName { get; set; } = string.Empty;
        public decimal? ProposedPrice { get; set; }
        public bool ProposedPriceOnArrangement { get; set; }
        public DateTime ProposedScheduledAtUtc { get; set; }
        public bool ProposedTimeFlexible { get; set; }
        public string JobProposalStatus { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime CreatedAtUtc { get; set; }

    }
}
