using AuthenticateAPI.Dto.Response;
using FluentValidation;

namespace AuthenticateAPI.FluentValidations.Dto.Response;

public class RegisteredDtoResponseValidator : AbstractValidator<RegisteredDtoResponse>
{
    public RegisteredDtoResponseValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Registration failed.")
            .When(x => !x.Success);

        RuleFor(x => x.Message)
            .Empty().WithMessage("Registration successful.")
            .When(x => x.Success);
    }
}