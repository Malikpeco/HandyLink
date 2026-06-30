using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewResponse> CreateReviewAsync(int jobId, ReviewInsertRequest request);
        Task<PageResult<ReviewResponse>> GetHandymanReviewsAsync(int handymanProfileId);
        Task<PageResult<ReviewResponse>> GetAdminReviewsAsync(ReviewSearchObject? search = null);

    }
}
