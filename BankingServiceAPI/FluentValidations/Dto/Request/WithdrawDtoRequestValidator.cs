using BankingServiceAPI.Dto.Request;
using FluentValidation;

namespace BankingServiceAPI.FluentValidations.Dto.Request;

public class WithdrawDtoRequestValidator : AbstractValidator<WithdrawDtoRequest>
{
    public WithdrawDtoRequestValidator()
    {
        RuleFor(x => x.AccountNumber)
            .GreaterThan(0)
            .WithMessage("Account number must be greater than 0.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0.");
    }
}