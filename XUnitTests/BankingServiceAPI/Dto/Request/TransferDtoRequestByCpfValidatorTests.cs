using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.FluentValidations.Dto.Request;
using FluentValidation.TestHelper;

namespace XUnitTests.BankingServiceAPI.Dto.Request;

public class TransferDtoRequestByCpfValidatorTests
{
     private readonly TransferDtoRequestByCpfValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_OriginCpf_Is_Empty()
        {
            // Arrange
            var model = new TransferDtoRequestByCpf("", "123.456.789-01", 10m);

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.OriginCpf)
                .WithErrorMessage("Origin CPF cannot be empty.");
        }

        [Theory]
        [InlineData("12345678900")] 
        [InlineData("123.456.78900")]
        public void Should_Have_Error_When_OriginCpf_Has_Invalid_Format(string originCpf)
        {
            // Arrange
            var model = new TransferDtoRequestByCpf(originCpf, "123.456.789-01", 10m);

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.OriginCpf)
                .WithErrorMessage("Invalid CPF format.");
        }

        [Fact]
        public void Should_Have_Error_When_DestinationCpf_Is_Empty()
        {
            // Arrange
            var model = new TransferDtoRequestByCpf("123.456.789-01", "", 10m);

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.DestinationCpf)
                .WithErrorMessage("Destination CPF cannot be empty.");
        }

        [Theory]
        [InlineData("12345678900")] 
        [InlineData("123.456.78900")]
        public void Should_Have_Error_When_DestinationCpf_Has_Invalid_Format(string destinationCpf)
        {
            // Arrange
            var model = new TransferDtoRequestByCpf("123.456.789-01", destinationCpf, 10m);

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.DestinationCpf)
                .WithErrorMessage("Invalid CPF format.");
        }

        [Fact]
        public void Should_Have_Error_When_Amount_Is_Not_Positive()
        {
            // Arrange
            var model = new TransferDtoRequestByCpf("123.456.789-01", "123.456.789-00", 0m);

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Transfer amount must be greater than zero.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
        {
            // Arrange
            var model = new TransferDtoRequestByCpf("123.456.789-01", "123.456.789-00", 10m);

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
}