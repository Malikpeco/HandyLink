using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Requests
{
    public class JobAddressUpdateRequest
    {
        public int ClientProfileId { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}
