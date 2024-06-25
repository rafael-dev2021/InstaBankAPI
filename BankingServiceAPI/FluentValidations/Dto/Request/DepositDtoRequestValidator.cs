using BankingServiceAPI.Dto.Request;
using FluentValidation;

namespace BankingServiceAPI.FluentValidations.Dto.Request;

public class DepositDtoRequestValidator: AbstractValidator<DepositDtoRequest>
{
    public DepositDtoRequestValidator()
    {
        RuleFor(x => x.AccountNumber)
            .GreaterThan(0)
            .WithMessage("Account number must be greater than zero");
        
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Transfer amount must be greater than zero.");
    }
}