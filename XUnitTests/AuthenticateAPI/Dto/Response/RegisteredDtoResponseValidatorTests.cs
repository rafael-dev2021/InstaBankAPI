using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.FluentValidations.Dto.Response;
using FluentValidation.TestHelper;

namespace XUnitTests.AuthenticateAPI.Dto.Response;

public class RegisteredDtoResponseValidatorTests
{
    private readonly RegisteredDtoResponseValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Registration_Is_Successful()
    {
        // Arrange
        var result = new RegisteredDtoResponse(true, string.Empty);

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.ErrorMessage);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Registration_Fails()
    {
        // Arrange
        var result = new RegisteredDtoResponse(false, "Registration failed.");

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.ErrorMessage);
    }

    [Fact]
    public void Should_Have_Error_When_ErrorMessage_Is_Not_Empty_On_Successful_Registration()
    {
        // Arrange
        var result = new RegisteredDtoResponse(true, "Some error message");

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.ErrorMessage)
            .WithErrorMessage("Registration successful.");
    }

    [Fact]
    public void Should_Have_Error_When_ErrorMessage_Is_Empty_On_Failed_Registration()
    {
        // Arrange
        var result = new RegisteredDtoResponse(false, string.Empty);

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.ErrorMessage)
            .WithErrorMessage("Registration failed.");
    }
}
