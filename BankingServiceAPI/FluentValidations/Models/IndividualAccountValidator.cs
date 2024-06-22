using BankingServiceAPI.Models;
using FluentValidation;

namespace BankingServiceAPI.FluentValidations.Models;

public class IndividualAccountValidator : AbstractValidator<IndividualAccount>
{
    public IndividualAccountValidator()
    {
        Include(new BaseEntityValidator());

        RuleFor(x => x.Cpf)
            .NotEmpty()
            .WithMessage("CPF is required.")
            .Matches(@"^\d{3}\.\d{3}\.\d{3}\-\d{2}$")
            .WithMessage("Invalid CPF format.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth is required.")
            .Must(date => date <= DateTime.Now.AddYears(-18))
            .WithMessage("Must be at least 18 years old.");

        RuleFor(x => x.Address)
            .NotNull()
            .WithMessage("Address is required.")
            .SetValidator(new AddressValidator()!);
    }
}