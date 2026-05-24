using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Responses
{
    public class HandymanProfileListResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public double AverageRating { get; set; }//set in service
        public int ExperienceYears { get; set; }//set in service
        public int JobsCompleted { get; set; } //set in service
        public List<HandymanServiceCategoryResponse> HandymanServiceCategories { get; set; } = new();
    }
}
