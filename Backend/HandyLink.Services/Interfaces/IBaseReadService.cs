using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;

namespace HandyLink.Services.Interfaces
{
    public interface IBaseReadService<TResponse, TSearchObject> 
        where TSearchObject : BaseSearchObject
    {
        Task<TResponse> GetByIdAsync(int id);
        Task<PageResult<TResponse>> GetAllAsync(TSearchObject? search = null);
    }
}
