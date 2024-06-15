using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Extensions;
using AuthenticateAPI.Models;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTests.AuthenticateAPI.Extensions;

public class FluentValidationDependencyInjectionTests
{
    [Fact]
    public void AddFluentValidationDependencyInjection_ShouldRegisterValidators()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMvc();

        // Act
        serviceCollection.AddFluentValidationDependencyInjection();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<IValidator<ChangePasswordDtoRequest>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<LoginDtoRequest>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<UpdateUserDtoRequest>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<AuthenticatedDtoResponse>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<RegisteredDtoResponse>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<User>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<RegisterDtoRequest>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<UpdatedDtoResponse>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<TokenDtoResponse>>());
    }
}