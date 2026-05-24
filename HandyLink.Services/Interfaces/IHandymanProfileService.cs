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
    public interface IHandymanProfileService
    {
        Task<HandymanProfileDetailsResponse> GetByIdAsync(int id);
        Task<PageResult<HandymanProfileListResponse>> GetAllAsync(HandymanProfileSearchObject? searchObject = null);
        Task<HandymanProfileDetailsResponse> InsertAsync(HandymanProfileInsertRequest request);
        Task<HandymanProfileDetailsResponse> UpdateAsync(int id, HandymanProfileUpdateRequest request);

    }
}
