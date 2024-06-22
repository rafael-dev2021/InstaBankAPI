using BankingServiceAPI.FluentValidations.Models;
using BankingServiceAPI.Models;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace BankingServiceAPI.Extensions;

public static class FluentValidationDependencyInjection
{
    public static void AddFluentValidationDependencyInjection(this IServiceCollection service)
    {
        service.AddValidatorsFromAssemblyContaining<BaseEntityValidator>();
        service.AddScoped<IValidator<BaseEntity>, BaseEntityValidator>();
        
        service.AddValidatorsFromAssemblyContaining<IndividualAccountValidator>();
        service.AddScoped<IValidator<IndividualAccount>, IndividualAccountValidator>();

        service.AddValidatorsFromAssemblyContaining<CorporateAccountValidator>();
        service.AddScoped<IValidator<CorporateAccount>, CorporateAccountValidator>();
        
        service.AddValidatorsFromAssemblyContaining<AddressValidator>();
        service.AddScoped<IValidator<Address>, AddressValidator>();
        
        service.AddFluentValidationAutoValidation();
        service.AddFluentValidationClientsideAdapters();
    }
}