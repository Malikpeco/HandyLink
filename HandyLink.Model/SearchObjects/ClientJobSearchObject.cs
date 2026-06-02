using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.SearchObjects
{
    public class ClientJobSearchObject : BaseSearchObject
    {
        public string? SearchTerm { get; set; }
        public int? JobStatusId { get; set; }

    }
}
