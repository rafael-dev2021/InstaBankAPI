using AuthenticateAPI.Dto.Response;
using FluentValidation;

namespace AuthenticateAPI.FluentValidations.Dto.Response;

public class AuthenticatedDtoResponseValidator : AbstractValidator<AuthenticatedDtoResponse>
{
    public AuthenticatedDtoResponseValidator()
    {
        RuleFor(x => x.Success)
            .Must(_ => true)
            .WithMessage("Invalid authentication state.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Invalid email or password. Please try again.")
            .When(x => !x.Success);

        RuleFor(x => x.Message)
            .Empty().WithMessage("Login successful.")
            .When(x => x.Success);
    }
}