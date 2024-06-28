using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.FluentValidations.Dto.Request;
using FluentValidation.TestHelper;

namespace XUnitTests.BankingServiceAPI.Dto.Request;

public class TransferDtoRequestByAccountValidatorTests
{
    private readonly TransferDtoRequestByAccountValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_OriginAccountNumber_Is_Not_Positive()
    {
        // Arrange
        var model = new TransferDtoRequestByAccount(-1234,1234,10m);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OriginAccountNumber)
            .WithErrorMessage("Origin account number must be greater than zero.");
    }

    [Fact]
    public void Should_Have_Error_When_DestinationAccountNumber_Is_Not_Positive()
    {
        // Arrange
        var model = new TransferDtoRequestByAccount(1234,-1234,10m);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DestinationAccountNumber)
            .WithErrorMessage("Destination account number must be greater than zero.");
    }

    [Fact]
    public void Should_Have_Error_When_Amount_Is_Not_Positive()
    {
        // Arrange
        var model = new TransferDtoRequestByAccount(1234,1234,0m);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage("Transfer amount must be greater than zero.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
    {
        // Arrange
        var model = new TransferDtoRequestByAccount(1234,1234,10m);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}