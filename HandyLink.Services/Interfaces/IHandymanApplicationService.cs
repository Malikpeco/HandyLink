using Azure;
using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Interfaces
{
    public interface IHandymanApplicationService 
    {
        Task<HandymanApplicationDetailsResponse> InsertAsync(HandymanApplicationInsertRequest request);

        Task<HandymanApplicationDetailsResponse> GetByIdAsync(int id);
        Task<PageResult<HandymanApplicationListResponse>> GetAllAsync(HandymanApplicationSearchObject? search = null);

        Task<HandymanApplicationDetailsResponse> SetDecisionAsync(int id, HandymanApplicationDecisionRequest request);


    }
}
