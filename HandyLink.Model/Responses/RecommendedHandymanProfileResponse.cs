using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Model.Responses
{
    public class RecommendedHandymanProfileResponse
    {
        public int HandymanProfileId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string? CityName { get; set; }
        public List<string> HandymanServiceCategoryNames { get; set; } = new();
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int JobsCompleted { get; set; }
        public decimal RecommendationScore { get; set; }
        public string Explanation { get; set; } = string.Empty;
    }
}
