using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.FluentValidations.Dto.Request;
using FluentValidation.TestHelper;

namespace XUnitTests.AuthenticateAPI.Dto.Request;

public class RefreshTokenDtoRequestValidatorTests
{
    private readonly RefreshTokenDtoRequestValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_RefreshToken_Is_Valid()
    {
        // Arrange
        var request = new RefreshTokenDtoRequest("valid_refresh_token");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_RefreshToken_Is_Null_Or_Empty(string? refreshToken)
    {
        // Arrange
        var request = new RefreshTokenDtoRequest(refreshToken);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
            .WithErrorMessage("Refresh token is required.");
    }
}