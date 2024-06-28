using BankingServiceAPI.Models;
using FluentValidation;

namespace BankingServiceAPI.FluentValidations.Models;

public class BankAccountValidator : AbstractValidator<BankAccount>
{
    public BankAccountValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than zero.");

        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Account number is required.")
            .Must(x => x.ToString().Length == 6).WithMessage("Account number must be 6 digits long.");

        RuleFor(x => x.Agency)
            .NotEmpty().WithMessage("Agency number is required.")
            .Must(x => x.ToString().Length == 4).WithMessage("Agency number must be 4 digits long.");

        RuleFor(x => x.Balance)
            .NotEmpty().WithMessage("Balance is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Balance must be a positive number.");

        RuleFor(x => x.AccountType)
            .IsInEnum().WithMessage("Invalid account type.");
    }
}