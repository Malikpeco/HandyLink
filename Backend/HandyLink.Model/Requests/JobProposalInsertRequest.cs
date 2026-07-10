using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Requests
{
    public class JobProposalInsertRequest
    {
        public int ProposedByUserId { get; set; }
        public int HandymanProfileId { get; set; }
        public decimal? ProposedPrice { get; set; }
        public bool ProposedPriceOnArrangement { get; set; }
        public DateTime ProposedScheduledAtUtc { get; set; }
        public bool ProposedTimeFlexible { get; set; }
        public string? Note { get; set; }
    }
}
