using HandyLink.Model.SearchObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
