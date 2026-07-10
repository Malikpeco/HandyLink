using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Requests
{
    public class JobProposalDecisionRequest
    {
        public int UserId { get; set; }
        public int JobProposalId { get; set; }
    }
}
