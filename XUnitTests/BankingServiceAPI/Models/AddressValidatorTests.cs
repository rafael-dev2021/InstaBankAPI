using BankingServiceAPI.FluentValidations.Models;
using BankingServiceAPI.Models;
using FluentValidation.TestHelper;

namespace XUnitTests.BankingServiceAPI.Models;

public class AddressValidatorTests
{
    private readonly AddressValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Address_Is_Valid()
    {
        // Arrange
        var address = new Address();
        address.SetStreet("street");
        address.SetNumber("123");
        address.SetCity("Valid City");
        address.SetState("Valid State");
        address.SetPostalCode("12345-678");
        address.SetCountry("Valid Country");

        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_Street_Is_Null_Or_Empty(string? street)
    {
        // Arrange
        var address = new Address();
        address.SetStreet(street);

        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Street)
            .WithErrorMessage("Street is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Street_Exceeds_MaxLength()
    {
        // Arrange
        var address = new Address();
        address.SetStreet(new string('a', 101));

        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Street)
            .WithErrorMessage("Street must not exceed 100 characters.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_Number_Is_Null_Or_Empty(string? number)
    {
        // Arrange
        var address = new Address();
        address.SetNumber(number);
        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Number)
            .WithErrorMessage("Number is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Number_Exceeds_MaxLength()
    {
        // Arrange
        var address = new Address();
        address.SetNumber(new string('a', 11));
        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Number)
            .WithErrorMessage("Number must not exceed 10 characters.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_City_Is_Null_Or_Empty(string? city)
    {
        // Arrange
        var address = new Address();
        address.SetCity(city);
        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City)
            .WithErrorMessage("City is required.");
    }

    [Fact]
    public void Should_Have_Error_When_City_Exceeds_MaxLength()
    {
        // Arrange
        var address = new Address();
        address.SetCity(new string('a', 51));
        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City)
            .WithErrorMessage("City must not exceed 50 characters.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_State_Is_Null_Or_Empty(string? state)
    {
        // Arrange
        var address = new Address();
        address.SetState(state);
        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.State)
            .WithErrorMessage("State is required.");
    }

    [Fact]
    public void Should_Have_Error_When_State_Exceeds_MaxLength()
    {
        // Arrange
        var address = new Address();
        address.SetState(new string('a', 51));
        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.State)
            .WithErrorMessage("State must not exceed 50 characters.");
    }
    
    [Theory]
    [InlineData("Valid Complement")]
    [InlineData("Short")]
    public void Should_Pass_Validation_For_Valid_Complement(string complement)
    {
        // Arrange
        var address = new Address();
        address.SetComplement(complement);

        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Complement);
    }
    
    [Fact]
    public void Should_Have_Error_When_Complement_Exceeds_MaxLength()
    {
        // Arrange
        var address = new Address();
        address.SetComplement(new string('a', 51));

        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Complement)
            .WithErrorMessage("Complement must not exceed 50 characters.");
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("12345678")]
    public void Should_Have_Error_When_PostalCode_Is_Invalid(string? postalCode)
    {
        // Arrange
        var address = new Address();
        address.SetPostalCode(postalCode);

        // Act
        var result = _validator.TestValidate(address);

        // Assert
        if (string.IsNullOrEmpty(postalCode))
        {
            result.ShouldHaveValidationErrorFor(x => x.PostalCode)
                .WithErrorMessage("Postal code is required.");
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.PostalCode)
                .WithErrorMessage("Invalid postal code format. Use XXXXX-XXX.");
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_Country_Is_Null_Or_Empty(string? country)
    {
        // Arrange
        var address = new Address();
        address.SetCountry(country);

        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country)
            .WithErrorMessage("Country is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Country_Exceeds_MaxLength()
    {
        // Arrange
        var address = new Address();
        address.SetCountry(new string('a', 51));

        // Act
        var result = _validator.TestValidate(address);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country)
            .WithErrorMessage("Country must not exceed 50 characters.");
    }
}