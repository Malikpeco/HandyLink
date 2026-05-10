using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Interfaces
{
    public interface IBaseReadService<TResponse, TSearchObject> 
        where TSearchObject : BaseSearchObject
    {
        Task<TResponse> GetByIdAsync(int id);
        Task<PageResult<TResponse>> GetAllAsync(TSearchObject? search = null);
    }
}
