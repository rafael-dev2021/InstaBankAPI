using BankingServiceAPI.Mapper;

namespace BankingServiceAPI.Extensions;

public static class MappingProfileDependencyInjection
{
    public static void AddMappingProfileDependencyInjection(this IServiceCollection service)
    {
        service.AddAutoMapper(typeof(MappingTheUserProfile));
        service.AddAutoMapper(typeof(MappingTheBankAccountProfile));
    }

}