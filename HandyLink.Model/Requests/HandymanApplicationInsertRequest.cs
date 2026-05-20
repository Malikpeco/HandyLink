using HandyLink.Model.Database.Enums;
using HandyLink.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Requests
{
    public class HandymanApplicationInsertRequest
    {
        public int UserId { get; set; }
        public int ExperienceYears { get; set; }
        public string WorkDescription { get; set; } = string.Empty;
        public List<int> ServiceCategoryIds { get; set; } = new();
        public List<HandymanApplicationPhotoInsertRequest> Photos { get; set; } = new();
        public List<HandymanApplicationDocumentInsertRequest> Documents { get; set; } = new();
        public List<HandymanApplicationReferenceInsertRequest> HandymanApplicationReferences { get; set; } = new();
    }
}
