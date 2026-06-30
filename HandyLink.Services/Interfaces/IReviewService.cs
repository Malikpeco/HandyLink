using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
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

    }
}
