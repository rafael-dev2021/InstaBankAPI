using AuthenticateAPI.Mapper;

namespace AuthenticateAPI.Extensions;

public static class MappingProfileDependencyInjection
{
    public static void AddMappingProfileDependencyInjection(this IServiceCollection service) =>
        service.AddAutoMapper(typeof(MappingTheUserProfile));
}