using BankingServiceAPI.Dto.Request;
using FluentValidation;

namespace BankingServiceAPI.FluentValidations.Dto.Request;

public class BankAccountDtoRequestValidator: AbstractValidator<BankAccountDtoRequest>
{
    public BankAccountDtoRequestValidator()
    {
        RuleFor(x => x.Balance)
            .NotEmpty().WithMessage("Balance is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Balance must be a positive number.");

        RuleFor(x => x.AccountType)
            .IsInEnum().WithMessage("Invalid account type.");
    }
}