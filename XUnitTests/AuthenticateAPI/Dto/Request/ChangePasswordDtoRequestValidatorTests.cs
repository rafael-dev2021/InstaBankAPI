using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.FluentValidations.Dto.Request;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace XUnitTests.AuthenticateAPI.Dto.Request;

public class ChangePasswordDtoRequestValidatorTests
{
    private readonly ChangePasswordDtoRequestValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_Email_Is_Null_Or_Empty(string? email)
    {
        // Arrange
        var request = new ChangePasswordDtoRequest(email, "OldPassword123!", "NewPassword123!");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        // Arrange
        var request = new ChangePasswordDtoRequest("invalid-email", "OldPassword123!", "NewPassword123!");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_OldPassword_Is_Null_Or_Empty(string? oldPassword)
    {
        // Arrange
        var request = new ChangePasswordDtoRequest("test@example.com", oldPassword, "NewPassword123!");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OldPassword)
            .WithErrorMessage("Old password is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_NewPassword_Is_Null_Or_Empty(string? newPassword)
    {
        // Arrange
        var request = new ChangePasswordDtoRequest("test@example.com", "OldPassword123!", newPassword);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password is required.");
    }

    [Theory]
    [InlineData("short1A!")]
    [InlineData("nouppercase123!")]
    [InlineData("NOLOWERCASE123!")]
    [InlineData("NoSpecialChar123")]
    [InlineData("NoDigit!")]
    public void Should_Have_Error_When_NewPassword_Is_Invalid(string? newPassword)
    {
        // Arrange
        var request = new ChangePasswordDtoRequest("test@example.com", "OldPassword123!", newPassword);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage(
                "Password must be between 10 and 30 characters, and include at least one digit, one lowercase letter, one uppercase letter, and one special character.");
    }

    [Fact]
    public void Should_Have_Error_When_NewPassword_Is_Same_As_OldPassword()
    {
        // Arrange
        var request = new ChangePasswordDtoRequest("test@example.com", "SamePassword123!", "SamePassword123!");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password must be different from old password.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        // Arrange
        var request = new ChangePasswordDtoRequest("test@example.com", "OldPassword123!", "NewPassword123!");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
