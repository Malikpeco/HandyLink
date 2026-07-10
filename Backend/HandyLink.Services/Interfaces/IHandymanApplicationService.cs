using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;

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
