using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.FluentValidations.Dto.Request;
using FluentValidation.TestHelper;

namespace XUnitTests.AuthenticateAPI.Dto.Request;

public class RegisterDtoRequestValidatorTests
{
    private readonly RegisterDtoRequestValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        // Arrange
        var request = new RegisterDtoRequest("John", "Doe", "+1234567890", "123.456.789-01", "john.doe@example.com", "Admin","StrongP@ssw0rd", "StrongP@ssw0rd");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_Name_Is_Null_Or_Empty(string? name)
    {
        // Arrange
        var request = new RegisterDtoRequest(name, "Doe", "+1234567890", "12345678901", "john.doe@example.com","Admin", "StrongP@ssw0rd", "StrongP@ssw0rd");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_LastName_Is_Null_Or_Empty(string? lastName)
    {
        // Arrange
        var request = new RegisterDtoRequest("John", lastName, "+1234567890", "12345678901", "john.doe@example.com", "Admin","StrongP@ssw0rd", "StrongP@ssw0rd");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required.");
    }

    [Theory]
    [InlineData("invalid-phone")]
    public void Should_Have_Error_When_PhoneNumber_Is_Invalid(string phoneNumber)
    {
        // Arrange
        var request = new RegisterDtoRequest("John", "Doe", phoneNumber, "12345678901", "john.doe@example.com", "Admin","StrongP@ssw0rd", "StrongP@ssw0rd");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("invalid-cpf")]
    public void Should_Have_Error_When_Cpf_Is_Invalid(string cpf)
    {
        // Arrange
        var request = new RegisterDtoRequest("John", "Doe", "+1234567890", cpf, "john.doe@example.com", "Admin","StrongP@ssw0rd", "StrongP@ssw0rd");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf);
    }

    [Theory]
    [InlineData("invalid-email")]
    public void Should_Have_Error_When_Email_Is_Invalid(string email)
    {
        // Arrange
        var request = new RegisterDtoRequest("John", "Doe", "+1234567890", "12345678901", email, "Admin","StrongP@ssw0rd", "StrongP@ssw0rd");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Passwords_Do_Not_Match()
    {
        // Arrange
        var request = new RegisterDtoRequest("John", "Doe", "+1234567890", "12345678901", "john.doe@example.com", "Admin","StrongP@ssw0rd", "DifferentP@ssw0rd");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword)
            .WithErrorMessage("Passwords do not match.");
    }
}