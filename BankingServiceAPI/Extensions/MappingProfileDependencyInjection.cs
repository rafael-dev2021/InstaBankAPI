using BankingServiceAPI.Mapper;

namespace BankingServiceAPI.Extensions;

public static class MappingProfileDependencyInjection
{
    public static void AddMappingProfileDependencyInjection(this IServiceCollection service)
    {
        service.AddAutoMapper(typeof(MappingTheUserProfile))
            .AddAutoMapper(typeof(MappingTheBankAccountProfile))
            .AddAutoMapper(typeof(MappingTheTransferProfile))
            .AddAutoMapper(typeof(MappingTheWithdrawProfile))
            .AddAutoMapper(typeof(MappingTheProfileDeposit));
    }
}