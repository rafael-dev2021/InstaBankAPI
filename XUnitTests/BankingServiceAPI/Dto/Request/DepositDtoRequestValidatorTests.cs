using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.FluentValidations.Dto.Request;
using FluentValidation.TestHelper;

namespace XUnitTests.BankingServiceAPI.Dto.Request;

public class DepositDtoRequestValidatorTests
{
    private readonly DepositDtoRequestValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_AccountNumber_Is_Less_Than_Or_Equal_To_Zero()
    {
        // Arrange
        var request = new DepositDtoRequest(0, 100m);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AccountNumber)
            .WithErrorMessage("Account number must be greater than zero");
    }

    [Fact]
    public void Should_Not_Have_Error_When_AccountNumber_Is_Greater_Than_Zero()
    {
        // Arrange
        var request = new DepositDtoRequest(1, 100m);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.AccountNumber);
    }

    [Fact]
    public void Should_Have_Error_When_Amount_Is_Less_Than_Or_Equal_To_Zero()
    {
        // Arrange
        var request = new DepositDtoRequest (1, 0m);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage("Transfer amount must be greater than zero.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Amount_Is_Greater_Than_Zero()
    {
        // Arrange
        var request = new DepositDtoRequest(1, 100m);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }
}