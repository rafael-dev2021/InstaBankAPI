using AuthenticateAPI.Dto.Request;
using FluentValidation;

namespace FluentValidations.AuthenticateAPI.Dto.Request;

public class UpdateUserDtoRequestValidator : AbstractValidator<UpdateUserDtoRequest>
{
    public UpdateUserDtoRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .Length(2, 50).WithMessage("Name must be between 2 and 50 characters long.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters long.");
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[0-9]{1,15}$")
            .WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
    }
}