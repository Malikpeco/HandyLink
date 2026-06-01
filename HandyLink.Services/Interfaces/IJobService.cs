using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Model.SearchObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Interfaces
{
    public interface IJobService
    {
        Task<JobDetailsResponse> CreateJobAsync(JobInsertRequest request);
        Task<JobDetailsResponse> GetByIdAsync(int id);
        Task<PageResult<JobListResponse>> GetAdminJobsAsync(AdminJobSearchObject? search = null);
        Task<JobDetailsResponse> AddCompletionMarkAsync(JobMarkRequest request);
        Task<JobDetailsResponse> AddCancellationMarkAsync(JobMarkRequest request);
        Task<JobDetailsResponse> RemoveCompletionMarkAsync(JobMarkRequest request);
        Task<JobDetailsResponse> RemoveCancellationMarkAsync(JobMarkRequest request);
        Task<JobDetailsResponse> InstantAcceptDirectProposalAsync(InstantAcceptDirectProposalRequest request);
        Task<JobDetailsResponse> InstantDeclineDirectProposalAsync(InstantDeclineDirectProposalRequest request);
        Task<JobProposalResponse> SuggestChangesAsync(int jobId, JobProposalInsertRequest request);
        Task<JobDetailsResponse> AcceptSuggestedChangesAsync(JobProposalDecisionRequest request);
        Task<JobDetailsResponse> DeclineSuggestedChangesAsync(JobProposalDecisionRequest request);
        Task<JobDetailsResponse> AddAddressAsync(int jobId, JobAddressUpdateRequest request);


    }
}
