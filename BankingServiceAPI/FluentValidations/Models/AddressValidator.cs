using BankingServiceAPI.Models;
using FluentValidation;

namespace BankingServiceAPI.FluentValidations.Models;

public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street is required.")
            .MaximumLength(100)
            .WithMessage("Street must not exceed 100 characters.");

        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage("Number is required.")
            .MaximumLength(10)
            .WithMessage("Number must not exceed 10 characters.");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required.")
            .MaximumLength(50)
            .WithMessage("City must not exceed 50 characters.");

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage("State is required.")
            .MaximumLength(50)
            .WithMessage("State must not exceed 50 characters.");

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .WithMessage("Postal code is required.")
            .Matches(@"^\d{5}-\d{3}$")
            .WithMessage("Invalid postal code format. Use XXXXX-XXX.");

        RuleFor(x => x.Complement)
            .MaximumLength(50)
            .WithMessage("Complement must not exceed 50 characters.");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required.")
            .MaximumLength(50)
            .WithMessage("Country must not exceed 50 characters.");
    }
}