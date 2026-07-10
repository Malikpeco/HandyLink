using HandyLink.Model.SearchObjects;

namespace HandyLink.Services.Interfaces
{
    public interface IBaseCRUDService<TResponse, TSearchObject, TInsertRequest, TUpdateRequest>
        :IBaseReadService<TResponse, TSearchObject>
       where TSearchObject : BaseSearchObject
    {
        Task<TResponse> InsertAsync(TInsertRequest request);
        Task<TResponse> UpdateAsync(int id, TUpdateRequest request);
        Task DeleteAsync(int id);
    }
}
