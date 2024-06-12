using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.FluentValidations.Dto.Response;
using FluentValidation.TestHelper;

namespace XUnitTests.AuthenticateAPI.Dto.Response;

public class TokenDtoResponseValidatorTests
{
    private readonly TokenDtoResponseValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Token_And_RefreshToken_Are_Valid()
    {
        // Arrange
        var result = new TokenDtoResponse("validToken123", "validRefreshToken123");

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Token);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Fact]
    public void Should_Have_Error_When_Token_Is_Empty()
    {
        // Arrange
        var result = new TokenDtoResponse(string.Empty, "validRefreshToken123");

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Token)
            .WithErrorMessage("Token is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Token_Is_Short()
    {
        // Arrange
        var result = new TokenDtoResponse("short", "validRefreshToken123");

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Token)
            .WithErrorMessage("Token must be at least 10 characters long.");
    }

    [Fact]
    public void Should_Have_Error_When_RefreshToken_Is_Empty()
    {
        // Arrange
        var result = new TokenDtoResponse("validToken123", string.Empty);

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.RefreshToken)
            .WithErrorMessage("Refresh token is required.");
    }

    [Fact]
    public void Should_Have_Error_When_RefreshToken_Is_Short()
    {
        // Arrange
        var result = new TokenDtoResponse("validToken123", "short");

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.RefreshToken)
            .WithErrorMessage("Refresh token must be at least 10 characters long.");
    }
}