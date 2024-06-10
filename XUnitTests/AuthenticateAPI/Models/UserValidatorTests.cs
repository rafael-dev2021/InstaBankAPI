using AuthenticateAPI.Models;
using FluentAssertions;
using FluentValidation.TestHelper;
using FluentValidations.AuthenticateAPI.Models;

namespace XUnitTests.AuthenticateAPI.Models;

public class UserValidatorTests
{
    private readonly UserValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_User_Is_Valid()
    {
        // Arrange
        var user = new User();
        user.SetName("John");
        user.SetLastName("Doe");
        user.SetEmail("john.doe@example.com");
        user.SetPhoneNumber("+1234567890");
        user.SetCpf("123.456.789-10");

        // Act
        var validationResult = _validator.TestValidate(user);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_Name_Is_Null_Or_Empty(string name)
    {
        // Arrange
        var user = new User();
        user.SetName(name);

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required.");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("This name is way too long and definitely exceeds fifty characters.")]
    public void Should_Have_Error_When_Name_Length_Is_Invalid(string name)
    {
        // Arrange
        var user = new User();
        user.SetName(name);

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name must be between 2 and 50 characters long.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_LastName_Is_Null_Or_Empty(string lastName)
    {
        // Arrange
        var user = new User();
        user.SetLastName(lastName);

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required.");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("This last name is way too long and definitely exceeds fifty characters.")]
    public void Should_Have_Error_When_LastName_Length_Is_Invalid(string lastName)
    {
        // Arrange
        var user = new User();
        user.SetLastName(lastName);

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name must be between 2 and 50 characters long.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_Email_Is_Null_Or_Empty(string email)
    {
        // Arrange
        var user = new User();
        user.SetEmail(email);

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");
    }

    [Theory]
    [InlineData("invalid-email")]
    public void Should_Have_Error_When_Email_Is_Invalid(string email)
    {
        // Arrange
        var user = new User();
        user.SetEmail(email);

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("123abc")]
    [InlineData("123-abc-456")]
    public void Should_Have_Error_When_PhoneNumber_Has_Letters(string phoneNumber)
    {
        // Arrange
        var user = new User();
        user.SetPhoneNumber(phoneNumber);

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Invalid phone number format.");
    }

    [Theory]
    [InlineData("1234567890")]
    [InlineData("+1234567890")]
    [InlineData("123456789012345")]
    public void Should_Not_Have_Error_When_PhoneNumber_Is_Valid(string phoneNumber)
    {
        // Arrange
        var user = new User();
        user.SetPhoneNumber(phoneNumber);

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("1234567890123456")]
    [InlineData("+1234567890123456")]
    public void Should_Have_Error_When_PhoneNumber_Is_Too_Long(string phoneNumber)
    {
        // Arrange
        var user = new User();
        user.SetPhoneNumber(phoneNumber);

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Invalid phone number format.");
    }

    [Theory]
    [InlineData("123.456.789-10")]
    [InlineData("987.654.321-00")]
    public void Should_Not_Have_Error_When_Cpf_Is_Valid(string cpf)
    {
        // Arrange
        var user = new User();
        user.SetCpf(cpf);

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Cpf);
    }

    [Theory]
    [InlineData("12345678910")]
    [InlineData("123.456.789-100")]
    [InlineData("123.4567.89-10")]
    public void Should_Have_Error_When_Cpf_Is_Invalid(string cpf)
    {
        // Arrange
        var user = new User();
        user.SetCpf(cpf);

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("Invalid CPF format.");
    }
}
