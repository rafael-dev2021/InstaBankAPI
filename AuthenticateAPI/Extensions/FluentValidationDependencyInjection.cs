using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.FluentValidations.Dto.Request;
using AuthenticateAPI.FluentValidations.Dto.Response;
using AuthenticateAPI.FluentValidations.Models;
using AuthenticateAPI.Models;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace AuthenticateAPI.Extensions;

public static class FluentValidationDependencyInjection
{
    public static void AddFluentValidationDependencyInjection(this IServiceCollection service)
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
        
        service.AddValidatorsFromAssemblyContaining<RegisterDtoRequestValidator>();
        service.AddScoped<IValidator<RegisterDtoRequest>, RegisterDtoRequestValidator>();
        
        service.AddValidatorsFromAssemblyContaining<UpdatedDtoResponseValidator>();
        service.AddScoped<IValidator<UpdatedDtoResponse>, UpdatedDtoResponseValidator>();
        
        service.AddValidatorsFromAssemblyContaining<TokenDtoResponseValidator>();
        
        service.AddValidatorsFromAssemblyContaining<ForgotPasswordDtoRequestValidator>();
        service.AddScoped<IValidator<ForgotPasswordDtoRequest>, ForgotPasswordDtoRequestValidator>();

        service.AddValidatorsFromAssemblyContaining<RefreshTokenDtoRequestValidator>();
        service.AddScoped<IValidator<RefreshTokenDtoRequest>, RefreshTokenDtoRequestValidator>();
        
        service.AddFluentValidationAutoValidation();
        service.AddFluentValidationClientsideAdapters();
    }
}