using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Requests
{
    public class RefreshAccessTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
