using FluentValidation;
using FluentValidation.Results;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Database;
using MapsterMapper;


namespace HandyLink.Services
{
    public abstract class BaseCRUDService<TEntity, TResponse, TSearchObject, TInsertRequest, TUpdateRequest>
        :BaseReadService<TEntity, TResponse, TSearchObject>
        where TEntity : class
        where TSearchObject : BaseSearchObject
    {
        protected readonly IValidator<TInsertRequest> _insertValidator;
        protected readonly IValidator<TUpdateRequest> _updateValidator;

        protected BaseCRUDService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<TInsertRequest> insertValidator, IValidator<TUpdateRequest> updateValidator) : base(mapper, dbContext)
        {
            _insertValidator = insertValidator;
            _updateValidator = updateValidator;
        }


        public virtual async Task<TResponse> InsertAsync(TInsertRequest request)
        {
            var validationResult = await _insertValidator.ValidateAsync(request);
            if(validationResult.IsValid == false)
            {
                var errors = validationResult.Errors.Select(e => _mapper.Map<ValidationFailure>(e));
                throw new ValidationException(errors);
            }

            var entity = MapInsertRequestToEntity(request);

            _dbContext.Set<TEntity>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(_mapper.Map<TResponse>(entity));
        }


        public virtual async Task<TResponse> UpdateAsync(int id, TUpdateRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if(validationResult.IsValid == false)
            {
                var errors = validationResult.Errors.Select(e => _mapper.Map<ValidationFailure>(e));
                throw new ValidationException(errors);
            }

            var entity = _dbContext.Set<TEntity>().Find(id);
            if (entity == null)
                throw new KeyNotFoundException($"{typeof(TEntity).Name} with id {id} not found");

            MapUpdateRequestToEntity(request, entity);

            await _dbContext.SaveChangesAsync();
            return await Task.FromResult(_mapper.Map<TResponse>(entity));
            
        }


        public virtual async Task DeleteAsync(int id)
        {
            var entity = _dbContext.Set<TEntity>().Find(id);

            if(entity == null)
                throw new KeyNotFoundException($"{typeof(TEntity).Name} with id {id} not found.");

            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }


        protected virtual TEntity MapInsertRequestToEntity(TInsertRequest request)
        {
            var entity = _mapper.Map<TEntity>(request ?? throw new ArgumentNullException(nameof(request)));
            return entity;
        }

        protected virtual void MapUpdateRequestToEntity(TUpdateRequest request, TEntity entity)
        {
            _mapper.Map(request, entity);
        }

    }
}
