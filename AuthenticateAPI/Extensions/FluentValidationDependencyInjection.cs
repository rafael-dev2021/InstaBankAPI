using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.FluentValidations.Dto.Request;
using AuthenticateAPI.FluentValidations.Dto.Response;
using AuthenticateAPI.FluentValidations.Models;
using AuthenticateAPI.Models;
using FluentValidation;

namespace AuthenticateAPI.Extensions;

public static class FluentValidationDependencyInjection
{
    public static IServiceCollection AddFluentValidationDependencyInjection(this IServiceCollection service)
    {
        service.AddValidatorsFromAssemblyContaining<ChangePasswordDtoRequestValidator>();
        service.AddScoped<IValidator<ChangePasswordDtoRequest>, ChangePasswordDtoRequestValidator>();
        
        service.AddValidatorsFromAssemblyContaining<LoginDtoRequestValidator>();
        service.AddScoped<IValidator<LoginDtoRequest>, LoginDtoRequestValidator>();
        
        service.AddValidatorsFromAssemblyContaining<UpdateUserDtoRequestValidator>();
        service.AddScoped<IValidator<UpdateUserDtoRequest>, UpdateUserDtoRequestValidator>();

        service.AddValidatorsFromAssemblyContaining<AuthenticatedDtoResponseValidator>();
        service.AddScoped<IValidator<AuthenticatedDtoResponse>, AuthenticatedDtoResponseValidator>();

        service.AddValidatorsFromAssemblyContaining<RegisteredDtoResponseValidator>();
        service.AddScoped<IValidator<RegisteredDtoResponse>, RegisteredDtoResponseValidator>();

        service.AddValidatorsFromAssemblyContaining<UserValidator>();
        service.AddScoped<IValidator<User>, UserValidator>();

        return service;
    }
}