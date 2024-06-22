using BankingServiceAPI.FluentValidations.Models;
using BankingServiceAPI.Models;
using FluentValidation.TestHelper;

namespace XUnitTests.BankingServiceAPI.Models;

public class BaseEntityValidatorTests
{
    private readonly BaseEntityValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Entity_Is_Valid()
    {
        // Arrange
        var entity = new BaseEntity();
        entity.SetId(new Guid("e8c1a4d4-9d45-4b8d-a5d6-3f1a48794b2f"));
        entity.SetAccountNumber("123456789");
        entity.SetAgency("1234");
        entity.SetBalance(1000.00m);

        // Act
        var result = _validator.TestValidate(entity);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_AccountNumber_Is_Null_Or_Empty(string? accountNumber)
    {
        // Arrange
        var entity = new BaseEntity();
        entity.SetAccountNumber(accountNumber);

        // Act
        var result = _validator.TestValidate(entity);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AccountNumber)
            .WithErrorMessage("Account number is required.");
    }

    [Theory]
    [InlineData("12345678")]  // Less than 9 digits
    [InlineData("1234567890")]  // More than 9 digits
    public void Should_Have_Error_When_AccountNumber_Is_Not_9_Digits(string accountNumber)
    {
        // Arrange
        var entity = new BaseEntity();
        entity.SetAccountNumber(accountNumber);

        // Act
        var result = _validator.TestValidate(entity);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AccountNumber)
            .WithErrorMessage("Account number must be 9 digits.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_Agency_Is_Null_Or_Empty(string? agency)
    {
        // Arrange
        var entity = new BaseEntity();
        entity.SetAgency(agency);

        // Act
        var result = _validator.TestValidate(entity);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Agency)
            .WithErrorMessage("Agency is required.");
    }

    [Theory]
    [InlineData("123")] 
    [InlineData("12345")]  
    public void Should_Have_Error_When_Agency_Is_Not_4_Digits(string agency)
    {
        // Arrange
        var entity = new BaseEntity();
        entity.SetAgency(agency);

        // Act
        var result = _validator.TestValidate(entity);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Agency)
            .WithErrorMessage("Agency must be 4 digits.");
    }

    [Theory]
    [InlineData(-1)]
    public void Should_Have_Error_When_Balance_Is_Negative(decimal balance)
    {
        // Arrange
        var entity = new BaseEntity();
        entity.SetBalance(balance);

        // Act
        var result = _validator.TestValidate(entity);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Balance)
            .WithErrorMessage("Balance must be greater than or equal to zero.");
    }
}
