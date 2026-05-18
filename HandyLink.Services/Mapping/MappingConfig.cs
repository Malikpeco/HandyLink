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
                .Map(dest => dest.UserType, src => src.UserType.ToString());
        }
    }
}
