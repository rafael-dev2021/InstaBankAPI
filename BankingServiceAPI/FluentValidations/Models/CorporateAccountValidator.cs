using BankingServiceAPI.Models;
using FluentValidation;

namespace BankingServiceAPI.FluentValidations.Models;

public class CorporateAccountValidator : AbstractValidator<CorporateAccount>
{
    public CorporateAccountValidator()
    {
        Include(new BaseEntityValidator());

        RuleFor(x => x.Cnpj)
            .NotEmpty()
            .WithMessage("CNPJ is required.")
            .Matches(@"^\d{2}\.\d{3}\.\d{3}\/\d{4}\-\d{2}$")
            .WithMessage("Invalid CNPJ format.");

        RuleFor(x => x.BusinessName)
            .NotEmpty()
            .WithMessage("Business name is required.")
            .MaximumLength(100)
            .WithMessage("Business name must not exceed 100 characters.");
    }
}