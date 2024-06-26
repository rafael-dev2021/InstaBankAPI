using BankingServiceAPI.Endpoints.Strategies;
using FluentValidation;

namespace XUnitTests.BankingServiceAPI.Endpoints.Strategies;

public class RequestValidatorTests
{
    private class TestRequest
    {
        public string Name { get; init; } = string.Empty;
    }

    private class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    [Fact]
    public async Task ValidateAsync_ValidRequest_ReturnsNull()
    {
        // Arrange
        var request = new TestRequest { Name = "Valid Name" };
        var validator = new TestRequestValidator();

        // Act
        var result = await RequestValidator.ValidateAsync(request, validator);

        // Assert
        Assert.Null(result);
    }
}