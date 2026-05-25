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
    public interface IClientProfileService
    {
        Task<ClientProfileDetailsResponse> GetByIdAsync(int id);
        //Task<PageResult<ClientProfileListResponse>> GetAllAsync(ClientProfileSearchObject? searchObject = null);
        //Task<ClientProfileDetailsResponse> InsertAsync(ClientProfileInsertRequest request);
        //Task<ClientProfileDetailsResponse> UpdateAsync(int id, ClientProfileUpdateRequest request);

    }
}
