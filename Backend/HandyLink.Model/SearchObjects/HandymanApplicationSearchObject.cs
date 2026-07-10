using HandyLink.Model.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.SearchObjects
{
    public class HandymanApplicationSearchObject : BaseSearchObject
    {
        public string? SearchTerm { get; set; }
        public HandymanApplicationStatus? Status { get; set; }

    }
}
