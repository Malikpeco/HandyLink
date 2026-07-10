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
    public abstract class BaseReadController<TResponse, TSearchObject, TService> : ControllerBase
        where TSearchObject : BaseSearchObject
        where TService : IBaseReadService<TResponse, TSearchObject>
    {
        protected readonly TService _service;

        protected BaseReadController(TService service)
        {
            _service = service;
        }

        [HttpGet]
        public virtual async Task<PageResult<TResponse>> GetAll([FromQuery] TSearchObject? searchObject)
        {
            var results = await _service.GetAllAsync(searchObject);
            return results;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TResponse>> GetById(int id)
        {
                var result = await _service.GetByIdAsync(id);
                return Ok(result);
        }


    }
}
