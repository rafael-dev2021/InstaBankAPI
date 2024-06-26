using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.FluentValidations.Dto.Request;
using FluentValidation.TestHelper;

namespace XUnitTests.BankingServiceAPI.Dto.Request;

public class WithdrawDtoRequestValidatorTests
{
    private readonly WithdrawDtoRequestValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_AccountNumber_Is_Less_Than_Or_Equal_To_Zero()
    {
        var request = new WithdrawDtoRequest(0,100m); 

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.AccountNumber);
    }

    [Fact]
    public void Should_Not_Have_Error_When_AccountNumber_Is_Greater_Than_Zero()
    {
        var request = new WithdrawDtoRequest(123,100m); 

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.AccountNumber);
    }

    [Fact]
    public void Should_Have_Error_When_Amount_Is_Less_Than_Or_Equal_To_Zero()
    {
        var request = new WithdrawDtoRequest(123,0m); 

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Amount_Is_Greater_Than_Zero()
    {
        var request = new WithdrawDtoRequest (1,100m); 

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }
}