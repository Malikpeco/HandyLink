using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
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
        //Task<HandymanProfileListResponse> GetAllAsync(int id);
        Task<HandymanProfileDetailsResponse> InsertAsync(HandymanProfileInsertRequest request);
    }
}
