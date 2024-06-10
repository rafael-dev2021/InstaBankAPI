using AuthenticateAPI.Dto.Request;
using FluentAssertions;
using FluentValidation.TestHelper;
using FluentValidations.AuthenticateAPI.Dto.Request;

namespace XUnitTests.AuthenticateAPI.Dto.Request;

public class LoginDtoRequestValidatorTests
{
    private readonly LoginDtoRequestValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        // Arrange
        var request = new LoginDtoRequest("test@example.com", "ValidPassword123!", true);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_Email_Is_Null_Or_Empty(string? email)
    {
        // Arrange
        var request = new LoginDtoRequest(email, "ValidPassword123!", true);

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
        var request = new LoginDtoRequest("invalid-email", "ValidPassword123!", true);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_Password_Is_Null_Or_Empty(string? password)
    {
        // Arrange
        var request = new LoginDtoRequest("test@example.com", password, true);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required.");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_Not_Have_Error_For_RememberMe_When_Valid(bool rememberMe)
    {
        // Arrange
        var request = new LoginDtoRequest("test@example.com", "ValidPassword123!", rememberMe);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RememberMe);
    }
}
