using BankingServiceAPI.FluentValidations.Models;
using BankingServiceAPI.Models;
using FluentValidation.TestHelper;

namespace XUnitTests.BankingServiceAPI.Models;

public class CorporateAccountValidatorTests
{
    private readonly CorporateAccountValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_Cnpj_Is_Null_Or_Empty(string? cnpj)
    {
        // Arrange
        var account = new CorporateAccount();
        account.SetCnpj(cnpj);

        // Act
        var result = _validator.TestValidate(account);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cnpj)
            .WithErrorMessage("CNPJ is required.");
    }

    [Theory]
    [InlineData("12345678901234")] 
    [InlineData("12.345.678/000199")] 
    [InlineData("12.345.678/000100")] 
    public void Should_Have_Error_When_Cnpj_Has_Invalid_Format(string cnpj)
    {
        // Arrange
        var account = new CorporateAccount();
        account.SetCnpj(cnpj);

        // Act
        var result = _validator.TestValidate(account);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cnpj)
            .WithErrorMessage("Invalid CNPJ format.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Cnpj_Is_Valid()
    {
        // Arrange
        var account = new CorporateAccount();
        account.SetCnpj("12.345.678/0001-01");

        // Act
        var result = _validator.TestValidate(account);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Cnpj);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_BusinessName_Is_Null_Or_Empty(string? businessName)
    {
        // Arrange
        var account = new CorporateAccount();
        account.SetBusinessName(businessName);

        // Act
        var result = _validator.TestValidate(account);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BusinessName)
            .WithErrorMessage("Business name is required.");
    }

    [Fact]
    public void Should_Have_Error_When_BusinessName_Exceeds_MaxLength()
    {
        // Arrange
        var account = new CorporateAccount();
        account.SetBusinessName(new string('a', 101));

        // Act
        var result = _validator.TestValidate(account);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BusinessName)
            .WithErrorMessage("Business name must not exceed 100 characters.");
    }

    [Fact]
    public void Should_Have_Error_When_BusinessAddress_Is_Null()
    {
        // Arrange
        var account = new CorporateAccount();
        account.SetBusinessAddress(null);

        // Act
        var result = _validator.TestValidate(account);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BusinessAddress)
            .WithErrorMessage("Business address is required.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_BusinessAddress_Is_Valid()
    {
        // Arrange
        var validAddress = new Address();
        validAddress.SetStreet("Street");
        validAddress.SetNumber("123");
        validAddress.SetCity("City");
        validAddress.SetState("State");
        validAddress.SetPostalCode("12345-678");
        validAddress.SetCountry("Country");

        var account = new CorporateAccount();
        account.SetBusinessAddress(validAddress);

        // Act
        var result = _validator.TestValidate(account);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BusinessAddress);
    }
}