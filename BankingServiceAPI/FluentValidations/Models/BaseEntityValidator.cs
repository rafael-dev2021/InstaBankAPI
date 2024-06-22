using BankingServiceAPI.Models;
using FluentValidation;

namespace BankingServiceAPI.FluentValidations.Models;

public class BaseEntityValidator : AbstractValidator<BaseEntity>
{
    public BaseEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("Id must be a valid GUID.");
        
        RuleFor(x => x.AccountNumber)
            .NotEmpty()
            .WithMessage("Account number is required.")
            .Matches(@"^\d{9}$")
            .WithMessage("Account number must be 9 digits.");

        RuleFor(x => x.Agency)
            .NotEmpty()
            .WithMessage("Agency is required.")
            .Matches(@"^\d{4}$")
            .WithMessage("Agency must be 4 digits.");

        RuleFor(x => x.Balance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Balance must be greater than or equal to zero.");
        
        RuleFor(x => x.Address)
            .NotNull()
            .WithMessage("Address is required.")
            .SetValidator(new AddressValidator()!);
    }
}