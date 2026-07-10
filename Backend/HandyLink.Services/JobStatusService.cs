using FluentValidation;
using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Database;
using HandyLink.Services.Database.Entities;
using HandyLink.Services.Interfaces;
using MapsterMapper;


namespace HandyLink.Services
{
    public class JobStatusService : BaseCRUDService<JobStatus, JobStatusResponse, JobStatusSearchObject, JobStatusInsertRequest, JobStatusUpdateRequest>, IJobStatusService
    {
        public JobStatusService(HandyLinkDbContext dbContext, IMapper mapper, IValidator<JobStatusInsertRequest> insertValidator, IValidator<JobStatusUpdateRequest> updateValidator) : base(dbContext, mapper, insertValidator, updateValidator)
        {
        }

        protected override IEnumerable<JobStatus> ApplyFilters(IEnumerable<JobStatus> query, JobStatusSearchObject? searchObject)
        {
            if (searchObject?.Name != null)
            {
                var normalized = searchObject.Name.Trim().ToLower();

                query = query.Where(x => x.Name.Trim().ToLower().Contains(normalized));
            }

            return query;
        }
    }
}
