using AuthenticateAPI.Dto.Response;
using FluentValidation;

namespace FluentValidations.AuthenticateAPI.Dto.Response;

public class AuthenticatedDtoResponseValidator : AbstractValidator<AuthenticatedDtoResponse>
{
    public AuthenticatedDtoResponseValidator()
    {
        RuleFor(x => x.IsAuthenticated)
            .Must(_ => true)
            .WithMessage("Invalid authentication state.");

        RuleFor(x => x.ErrorMessage)
            .NotEmpty().WithMessage("Invalid email or password. Please try again.")
            .When(x => !x.IsAuthenticated);

        RuleFor(x => x.ErrorMessage)
            .Empty().WithMessage("Login successful.")
            .When(x => x.IsAuthenticated);
    }
}