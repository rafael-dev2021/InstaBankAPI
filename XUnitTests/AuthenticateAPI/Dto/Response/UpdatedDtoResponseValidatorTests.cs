using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.FluentValidations.Dto.Response;
using FluentValidation.TestHelper;

namespace XUnitTests.AuthenticateAPI.Dto.Response;

public class UpdatedDtoResponseValidatorTests
{
    private readonly UpdatedDtoResponseValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Update_Is_Successful()
    {
        // Arrange
        var response = new UpdatedDtoResponse(true, string.Empty);

        // Act
        var validationResult = _validator.TestValidate(response);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Update_Fails_With_Valid_Message()
    {
        // Arrange
        var response = new UpdatedDtoResponse(false, "Update failed.");

        // Act
        var validationResult = _validator.TestValidate(response);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Should_Have_Error_When_ErrorMessage_Is_Not_Empty_On_Successful_Update()
    {
        // Arrange
        var response = new UpdatedDtoResponse(true, "Some error message");

        // Act
        var validationResult = _validator.TestValidate(response);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Message)
            .WithErrorMessage("Update successful.");
    }

    [Fact]
    public void Should_Have_Error_When_ErrorMessage_Is_Empty_On_Failed_Update()
    {
        // Arrange
        var response = new UpdatedDtoResponse(false, string.Empty);

        // Act
        var validationResult = _validator.TestValidate(response);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Message)
            .WithErrorMessage("Update failed.");
    }

    [Fact]
    public void Should_Have_Error_When_ErrorMessage_Is_Null_On_Failed_Update()
    {
        // Arrange
        var response = new UpdatedDtoResponse(false, null);

        // Act
        var validationResult = _validator.TestValidate(response);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Message)
            .WithErrorMessage("Update failed.");
    }
}