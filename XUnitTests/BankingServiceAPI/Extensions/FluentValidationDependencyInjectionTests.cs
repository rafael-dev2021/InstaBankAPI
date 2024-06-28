using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Extensions;
using BankingServiceAPI.FluentValidations.Dto.Request;
using BankingServiceAPI.FluentValidations.Models;
using BankingServiceAPI.Models;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTests.BankingServiceAPI.Extensions;

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
        Assert.NotNull(serviceProvider.GetService<IValidator<BankAccount>>());
        Assert.IsType<BankAccountValidator>(serviceProvider.GetService<IValidator<BankAccount>>());

        Assert.NotNull(serviceProvider.GetService<IValidator<BankAccountDtoRequest>>());
        Assert.IsType<BankAccountDtoRequestValidator>(serviceProvider.GetService<IValidator<BankAccountDtoRequest>>());
    }
}