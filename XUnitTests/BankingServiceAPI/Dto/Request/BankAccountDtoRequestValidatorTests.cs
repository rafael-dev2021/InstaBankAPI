using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.FluentValidations.Dto.Request;
using BankingServiceAPI.Models;
using FluentValidation.TestHelper;

namespace XUnitTests.BankingServiceAPI.Dto.Request;

public class BankAccountDtoRequestValidatorTests
{
    private readonly BankAccountDtoRequestValidator _validator= new();

    [Fact]
    public void Should_Have_Error_When_Balance_Is_Negative()
    {
        // Arrange
        var model = new BankAccountDtoRequest(-1, AccountType.Savings);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Balance)
            .WithErrorMessage("Balance must be a positive number.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Balance_Is_Positive()
    {
        // Arrange
        var model = new BankAccountDtoRequest(10, AccountType.Savings);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Balance);
    }

    [Fact]
    public void Should_Not_Have_Error_When_AccountType_Is_Valid()
    {
        // Arrange
        var model = new BankAccountDtoRequest(10, AccountType.Current);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.AccountType);
    }
}