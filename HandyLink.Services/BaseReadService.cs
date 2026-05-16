using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Database;
using HandyLink.Services.Exceptions;
using HandyLink.Services.Interfaces;
using System.Linq.Dynamic.Core;



namespace HandyLink.Services
{
    public abstract class BaseReadService<TEntity, TResponse, TSearchObject> : IBaseReadService<TResponse, TSearchObject>
        where TEntity : class
        where TSearchObject : BaseSearchObject
    {
        protected readonly MapsterMapper.IMapper _mapper;
        protected readonly HandyLinkDbContext _dbContext;

        protected BaseReadService(MapsterMapper.IMapper mapper, HandyLinkDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }




        public virtual async Task<PageResult<TResponse>> GetAllAsync(TSearchObject? searchObject = null)
        {
            IEnumerable<TEntity> query = _dbContext.Set<TEntity>();

            query = await IncludeRelatedEntitiesAsync(query.AsQueryable(), searchObject);
            query = ApplyFilters(query, searchObject);

            int? totalCount = null;

            if (searchObject != null)
            {
                if (searchObject.IncludeTotalCount)
                {
                    totalCount = query.Count();
                }
                if (!string.IsNullOrWhiteSpace(searchObject.SortBy))
                {
                    query = query.AsQueryable().OrderBy(searchObject.SortBy);
                }
                query = query.Skip((searchObject.Page - 1) * searchObject.PageSize);
                query = query.Take(searchObject.PageSize);

            }
            
            var list = query.Select(item => _mapper.Map<TResponse>(item)).ToList();
                
            var pageResult = new PageResult<TResponse>
            {
                Items = list,
                TotalCount = totalCount,
            };

            return await Task.FromResult(pageResult);



        }

        public virtual async Task<TResponse> GetByIdAsync(int id)
        {
            var entity = _dbContext.Set<TEntity>().Find(id);

            if(entity == null)
            {
                throw new HandyLinkNotFoundException($"{typeof(TEntity).Name} with id {id} not found.");
            }

            return await Task.FromResult(_mapper.Map<TResponse>(entity));
        }








        protected abstract IEnumerable<TEntity> ApplyFilters(IEnumerable<TEntity> query, TSearchObject? searchObject);

        protected virtual async Task<IQueryable<TEntity>> IncludeRelatedEntitiesAsync(IQueryable<TEntity> query, TSearchObject? searchObject)
        {
            return query;
        }
    }
}
