using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Requests
{
    public class InstantAcceptDirectProposalRequest
    {
        public int JobId { get; set; }
        public int HandymanProfileId { get; set; }
    }
}
