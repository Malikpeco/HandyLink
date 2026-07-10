using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Requests
{
    public class HandymanProfileUpdateRequest
    {
        public string? Bio {  get; set; }
        public List<HandymanWorkPhotoInsertRequest>? HandymanWorkPhotos { get; set; }
        public List<int>? ServiceCategoryIds { get; set; }

    }
}
