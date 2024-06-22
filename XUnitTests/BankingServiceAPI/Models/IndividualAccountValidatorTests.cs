using BankingServiceAPI.FluentValidations.Models;
using BankingServiceAPI.Models;
using FluentValidation.TestHelper;

namespace XUnitTests.BankingServiceAPI.Models;

public class IndividualAccountValidatorTests
{
    private readonly IndividualAccountValidator _validator = new();

    [Theory]
    [InlineData("123.456.789-00")]
    [InlineData("987.654.321-00")]
    public void Should_Not_Have_Error_When_Cpf_Is_Valid(string cpf)
    {
        // Arrange
        var account = new IndividualAccount();
        account.SetId(new Guid("e8c1a4d4-9d45-4b8d-a5d6-3f1a48794b2f"));
        account.SetCpf(cpf);
        account.SetDateOfBirth(DateTime.Now.AddYears(-20));
        account.SetAccountNumber("123456789");
        account.SetAgency("1234");
        account.SetBalance(1000.00m);
        var address = new Address();
        address.SetStreet("street");
        address.SetNumber("123");
        address.SetCity("Valid City");
        address.SetState("Valid State");
        address.SetPostalCode("12345-678");
        address.SetCountry("Valid Country");
        account.SetAddress(address);

        // Act
        var result = _validator.TestValidate(account);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }


    [Theory]
    [InlineData(null, "CPF is required.")]
    [InlineData("", "CPF is required.")]
    [InlineData("123.456.789-0", "Invalid CPF format.")]
    [InlineData("123.456.789-000", "Invalid CPF format.")]
    [InlineData("12345678900", "Invalid CPF format.")]
    public void Should_Have_Error_When_Cpf_Has_Invalid_Format(string? cpf, string expectedErrorMessage)
    {
        // Arrange
        var account = new IndividualAccount();
        account.SetCpf(cpf);
        account.SetDateOfBirth(DateTime.Now.AddYears(-20));
        account.SetAddress(new Address());

        // Act
        var result = _validator.TestValidate(account);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage(expectedErrorMessage);
    }

    [Fact]
    public void Should_Have_Error_When_DateOfBirth_Is_Null()
    {
        // Arrange
        var account = new IndividualAccount();
        account.SetCpf("123.456.789-00");
        account.SetAddress(new Address());

        // Act
        var result = _validator.TestValidate(account);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
            .WithErrorMessage("Date of birth is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Under_18_Years_Old()
    {
        // Arrange
        var account = new IndividualAccount();
        account.SetCpf("123.456.789-00");
        account.SetDateOfBirth(DateTime.Now.AddYears(-17));
        account.SetAddress(new Address());

        // Act
        var result = _validator.TestValidate(account);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
            .WithErrorMessage("Must be at least 18 years old.");
    }

    [Fact]
    public void Should_Have_Error_When_Address_Is_Null()
    {
        // Arrange
        var account = new IndividualAccount();
        account.SetCpf("123.456.789-00");
        account.SetDateOfBirth(DateTime.Now.AddYears(-20));

        // Act
        var result = _validator.TestValidate(account);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address)
            .WithErrorMessage("Address is required.");
    }
}