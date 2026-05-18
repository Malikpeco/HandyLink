using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using HandyLink.Services.Interfaces;

namespace HandyLink.WebApi.Controllers
{
    public class JobStatusesController : BaseCRUDController<JobStatusResponse, JobStatusSearchObject, JobStatusInsertRequest, JobStatusUpdateRequest, IJobStatusService>
    {
        public JobStatusesController(IJobStatusService jobStatusService) : base(jobStatusService)
        {
        }
    }
}
