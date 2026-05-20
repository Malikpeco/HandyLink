using HandyLink.Model.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Responses
{
    public class HandymanApplicationDetailsResponse
    {
        public UserResponse User { get; set; } = null!;
        public int ExperienceYears { get; set; }
        public string WorkDescription { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<HandymanApplicationServiceCategoryResponse> HandymanApplicationServiceCategories { get; set; } = new();
        public List<HandymanApplicationPhotoResponse> HandymanApplicationPhotos { get; set; } = new();
        public List<HandymanApplicationDocumentResponse> HandymanApplicationDocuments { get; set; } = new();
        public List<HandymanApplicationReferenceResponse> HandymanApplicationReferences { get; set; } = new();
    }
}
