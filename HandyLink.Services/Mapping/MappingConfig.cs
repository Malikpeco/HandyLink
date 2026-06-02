using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Services.Database.Entities;
using Mapster;


namespace HandyLink.Services.Mapping
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<User, UserResponse>
                .NewConfig()
                .Map(dest => dest.UserType, src => src.UserType.ToString())
                .Map(dest => dest.Status, src => src.UserStatus);

            TypeAdapterConfig<HandymanApplication, HandymanApplicationListResponse>
                .NewConfig()
                .Map(dest => dest.Status, src => src.Status.ToString())
                .Map(dest => dest.SentAt, src => src.CreatedAtUtc);

            TypeAdapterConfig<HandymanApplication, HandymanApplicationDetailsResponse>
                .NewConfig()
                .Map(dest => dest.Status, src => src.Status.ToString());

            TypeAdapterConfig<HandymanApplicationInsertRequest, HandymanApplication>
                .NewConfig()
                .Map(dest => dest.HandymanApplicationServiceCategories,
                    src => src.ServiceCategoryIds
                    .Select(scId =>
                    new HandymanApplicationServiceCategory
                    {
                        ServiceCategoryId = scId
                    }).ToList())

                .Map(dest => dest.HandymanApplicationPhotos,
                src => src.Photos.Select(photo =>
                    new HandymanApplicationPhoto
                    {
                        ImageBase64 = photo.ImageBase64
                    }).ToList())

                .Map(dest => dest.HandymanApplicationDocuments,
                    src => src.Documents.Select(document =>
                        new HandymanApplicationDocument
                        {
                            FileUrl = document.FileUrl,
                            FileName = document.FileName,
                            ContentType = document.ContentType
                        }).ToList())

                .Map(dest => dest.HandymanApplicationReferences,
                    src => src.HandymanApplicationReferences.Select(reference =>
                        new HandymanApplicationReference
                        {
                            FirstName = reference.FirstName,
                            LastName = reference.LastName,
                            Email = reference.Email,
                            PhoneNumber = reference.PhoneNumber,
                            ReferenceNote = reference.ReferenceNote
                        }).ToList());

            TypeAdapterConfig<HandymanApplicationServiceCategory, HandymanApplicationServiceCategoryResponse>
                .NewConfig()
                .Map(dest => dest.ServiceCategoryId, src => src.ServiceCategoryId)
                .Map(dest => dest.ServiceCategoryName, src => src.ServiceCategory.Name);

            TypeAdapterConfig<HandymanProfile, HandymanProfileListResponse>
                .NewConfig()
                .Map(dest => dest.UserFullName, src => (src.User.FirstName + " " + src.User.LastName))
                .Map(dest => dest.AverageRating, src => src.Reviews.Count==0?0:src.Reviews.Average(x => x.Rating))
                .Map(dest => dest.JobsCompleted, src => src.Jobs.Where(x => x.JobStatus.Code == "COMPLETED").Count())
                .Map(dest => dest.CityName, src => src.User.City.Name)
                .Map(dest => dest.ProfileImageBase64, src => src.User.ProfileImageBase64)
                .Map(dest => dest.HandymanServiceCategories, src => src.HandymanServiceCategories);

            TypeAdapterConfig<Review, ReviewResponse>
                .NewConfig()
                .Map(dest => dest.JobTitle, src => src.Job.Title)
                .Map(dest => dest.ClientFullName, src => src.ClientProfile.User.FirstName + " " + src.ClientProfile.User.LastName)
                .Map(dest => dest.HandymanFullName, src => src.HandymanProfile.User.FirstName + " " + src.HandymanProfile.User.LastName);

            TypeAdapterConfig<HandymanServiceCategory, HandymanServiceCategoryResponse>
                .NewConfig()
                .Map(dest => dest.ServiceCategoryId, src => src.ServiceCategoryId)
                .Map(dest => dest.ServiceCategoryName, src => src.ServiceCategory.Name);

            TypeAdapterConfig<HandymanProfileUpdateRequest, HandymanProfile>
                .NewConfig()
                .Ignore(dest => dest.HandymanWorkPhotos);
            
            TypeAdapterConfig<ClientProfile, ClientProfileResponse>
                .NewConfig()
                .Map(dest => dest.User, src => src.User)
                .Map(dest => dest.CompletedJobs, src => src.Jobs.Where(x => x.JobStatus.Code == "COMPLETED").Count())
                .Map(dest => dest.ReviewsCount, src => src.Reviews.Count());


            TypeAdapterConfig<Job, JobDetailsResponse>
                .NewConfig()
                .Map(dest => dest.ClientFullName, src => src.ClientProfile.User.FirstName + " " + src.ClientProfile.User.LastName)
                .Map(dest => dest.HandymanFullName, src => src.HandymanProfile != null ? src.HandymanProfile.User.FirstName + " " + src.HandymanProfile.User.LastName : null)
                .Map(dest => dest.ServiceCategoryName, src => src.ServiceCategory.Name)
                .Map(dest => dest.CityName, src => src.City.Name)
                .Map(dest => dest.JobCreationType, src => src.JobCreationType.ToString())
                .Map(dest => dest.JobStatusName, src => src.JobStatus.Name);
         
            TypeAdapterConfig<Job, JobListResponse>
                .NewConfig()
                .Map(dest => dest.ClientFullName, src => src.ClientProfile.User.FirstName + " " + src.ClientProfile.User.LastName)
                .Map(dest => dest.HandymanFullName, src => src.HandymanProfile != null ? src.HandymanProfile.User.FirstName + " " + src.HandymanProfile.User.LastName : null)
                .Map(dest => dest.ServiceCategoryName, src => src.ServiceCategory.Name)
                .Map(dest => dest.CityName, src => src.City.Name)
                .Map(dest => dest.JobCreationType, src => src.JobCreationType.ToString())
                .Map(dest => dest.JobStatusName, src => src.JobStatus.Name);


            TypeAdapterConfig<JobProposal, JobProposalResponse>
                .NewConfig()
                .Map(dest => dest.JobTitle, src => src.Job.Title)
                .Map(dest => dest.ProposedByUserFullName, src => src.ProposedByUser.FirstName + " " + src.ProposedByUser.LastName)
                .Map(dest => dest.HandymanFullname, src => src.HandymanProfile.User.FirstName + " " + src.HandymanProfile.User.LastName)
                .Map(dest => dest.JobProposalStatus, src => src.JobProposalStatus.ToString());
            
            TypeAdapterConfig<JobCompletionMark, JobCompletionMarkResponse>
                .NewConfig()
                .Map(dest => dest.JobTitle, src => src.Job.Title)
                .Map(dest => dest.MarkedByUserFullName, src => src.MarkedByUser.FirstName + " " + src.MarkedByUser.LastName);
            
            TypeAdapterConfig<JobCancellationMark, JobCancellationMarkResponse>
                .NewConfig()
                .Map(dest => dest.JobTitle, src => src.Job.Title)
                .Map(dest => dest.MarkedByUserFullName, src => src.MarkedByUser.FirstName + " " + src.MarkedByUser.LastName);



        }
    }
}
