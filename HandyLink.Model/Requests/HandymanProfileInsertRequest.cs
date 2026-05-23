using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Requests
{
    public class HandymanProfileInsertRequest
    {
        public int UserId { get; set; }
        public string Bio { get; set; } = string.Empty;

        public List<HandymanWorkPhotoInsertRequest> HandymanWorkPhotos { get; set; } = new();
    }
}
