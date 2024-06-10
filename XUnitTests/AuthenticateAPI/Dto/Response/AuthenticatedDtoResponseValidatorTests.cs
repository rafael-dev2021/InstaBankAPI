using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.FluentValidations.Dto.Response;
using FluentValidation.TestHelper;

namespace XUnitTests.AuthenticateAPI.Dto.Response;

public class AuthenticatedDtoResponseValidatorTests
{
    private readonly AuthenticatedDtoResponseValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Authentication_Is_Successful()
    {
        // Arrange
        var result = new AuthenticatedDtoResponse(true, string.Empty);

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.ErrorMessage);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Authentication_Fails()
    {
        // Arrange
        var result = new AuthenticatedDtoResponse(false, "Invalid email or password. Please try again.");

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.ErrorMessage);
    }

    [Fact]
    public void Should_Have_Error_When_ErrorMessage_Is_Not_Empty_On_Successful_Authentication()
    {
        // Arrange
        var result = new AuthenticatedDtoResponse(true, "Some error message");

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.ErrorMessage)
            .WithErrorMessage("Login successful.");
    }

    [Fact]
    public void Should_Have_Error_When_ErrorMessage_Is_Empty_On_Failed_Authentication()
    {
        // Arrange
        var result = new AuthenticatedDtoResponse(false, string.Empty);

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.ErrorMessage)
            .WithErrorMessage("Invalid email or password. Please try again.");
    }
}
