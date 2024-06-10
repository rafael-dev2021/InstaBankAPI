using AuthenticateAPI.Dto.Request;
using FluentAssertions;
using FluentValidation.TestHelper;
using FluentValidations.AuthenticateAPI.Dto.Request;

namespace XUnitTests.AuthenticateAPI.Dto.Request;

public class UpdateUserDtoRequestValidatorTests
{
    private readonly UpdateUserDtoRequestValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        // Arrange
        var request = new UpdateUserDtoRequest("John", "Doe", "john.doe@example.com", "+1234567890");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_Name_Is_Null_Or_Empty(string? name)
    {
        // Arrange
        var request = new UpdateUserDtoRequest(name, "Doe", "john.doe@example.com", "+1234567890");

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
        var request = new UpdateUserDtoRequest("John", lastName, "john.doe@example.com", "+1234567890");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required.");
    }

    [Theory]
    [InlineData("j")]
    [InlineData("a very very very very very very very long name that exceeds fifty characters")]
    public void Should_Have_Error_When_Name_Is_Invalid(string name)
    {
        // Arrange
        var request = new UpdateUserDtoRequest(name, "Doe", "john.doe@example.com", "+1234567890");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("j")]
    [InlineData("a very very very very very very very long last name that exceeds fifty characters")]
    public void Should_Have_Error_When_LastName_Is_Invalid(string lastName)
    {
        // Arrange
        var request = new UpdateUserDtoRequest("John", lastName, "john.doe@example.com", "+1234567890");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Theory]
    [InlineData("invalid-email")]
    public void Should_Have_Error_When_Email_Is_Invalid(string email)
    {
        // Arrange
        var request = new UpdateUserDtoRequest("John", "Doe", email, "+1234567890");

        // Act
        var result = _validator.TestValidate(request);

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
        var request = new UpdateUserDtoRequest("John", "Doe", "john.doe@example.com", phoneNumber);

        // Act
        var result = _validator.TestValidate(request);

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
        var request = new UpdateUserDtoRequest("John", "Doe", "john.doe@example.com", phoneNumber);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("1234567890123456")]
    [InlineData("+1234567890123456")]
    public void Should_Have_Error_When_PhoneNumber_Is_Too_Long(string phoneNumber)
    {
        // Arrange
        var request = new UpdateUserDtoRequest("John", "Doe", "john.doe@example.com", phoneNumber);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Invalid phone number format.");
    }
}
