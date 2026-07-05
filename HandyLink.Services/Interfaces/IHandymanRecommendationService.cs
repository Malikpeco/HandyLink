using HandyLink.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Interfaces
{
    public interface IHandymanRecommendationService
    {
        Task<IReadOnlyList<RecommendedHandymanProfileResponse>> RecommendAsync(int clientProfileId, int take = 3, CancellationToken cancellationToken = default);
        
    }
}
