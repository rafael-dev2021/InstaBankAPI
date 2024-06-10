using AuthenticateAPI.Dto.Response;
using FluentValidation;

namespace AuthenticateAPI.FluentValidations.Dto.Response;

public class RegisteredDtoResponseValidator : AbstractValidator<RegisteredDtoResponse>
{
    public RegisteredDtoResponseValidator()
    {
        RuleFor(x => x.ErrorMessage)
            .NotEmpty().WithMessage("Registration failed.")
            .When(x => !x.IsRegistered);

        RuleFor(x => x.ErrorMessage)
            .Empty().WithMessage("Registration successful.")
            .When(x => x.IsRegistered);
    }
}