using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HandyLink.WebApi.Controllers
{
    //add [Authorize] later


    [ApiController]
    [Route("[controller]")]
    public abstract class BaseCRUDController<TResponse, TSearchObject, TInsertRequest, TUpdateRequest, TService> : BaseReadController<TResponse,TSearchObject, TService>
        where TSearchObject : BaseSearchObject
        where TService : IBaseCRUDService<TResponse, TSearchObject, TInsertRequest, TUpdateRequest>
    {
        protected BaseCRUDController(TService service) : base(service)
        {
        }

        [HttpPost]
        public async Task<ActionResult<TResponse>> Create([FromBody] TInsertRequest request)
        {
            var result = await _service.InsertAsync(request);
            return result;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TResponse>> Update(int id, [FromBody] TUpdateRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return result;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

    }
}
