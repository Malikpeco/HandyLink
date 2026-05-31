using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
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
        Task<JobDetailsResponse> AddCompletionMarkAsync(JobMarkRequest request);
        Task<JobDetailsResponse> AddCancellationMarkAsync(JobMarkRequest request);
        Task<JobDetailsResponse> AcceptDirectProposalAsync(AcceptDirectProposalRequest request);
        Task<JobDetailsResponse> DeclineDirectProposalAsync(DeclineDirectProposalRequest request);
        Task<JobProposalResponse> SuggestChangesAsync(int id, JobProposalInsertRequest request);


    }
}
