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
                .Map(dest => dest.HandymanServiceCategories, src => src.HandymanServiceCategories);

            TypeAdapterConfig<HandymanServiceCategory, HandymanServiceCategoryResponse>
                .NewConfig()
                .Map(dest => dest.ServiceCategoryId, src => src.ServiceCategoryId)
                .Map(dest => dest.ServiceCategoryName, src => src.ServiceCategory.Name);



        }
    }
}
