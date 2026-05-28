using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Responses
{
    public class HandymanProfileDetailsResponse 
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserResponse User { get; set; } = null!;
        public string Bio { get; set; } = string.Empty;
        public List<HandymanServiceCategoryResponse> HandymanServiceCategories { get; set; } = new();
        public List<HandymanWorkPhotoResponse> HandymanWorkPhotos { get; set; } = new();
        public List<ReviewResponse> Reviews { get; set; } = new();
        public int ExperienceYears { get; set; }
        public int JobsCompleted { get; set; } //set in service
        public double AverageRating { get; set; }//set in service
        public int ReviewsCount { get; set; } //set in service
    }
}
