using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Requests
{
    public class MessageInsertRequest
    {
        public int SenderUserId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
