using AuthenticateAPI.Dto.Request;
using FluentValidation;

namespace AuthenticateAPI.FluentValidations.Dto.Request;

public class ChangePasswordDtoRequestValidator : AbstractValidator<ChangePasswordDtoRequest>
{
    public ChangePasswordDtoRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithMessage("Old password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .Matches(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=!])(?=\S+$).{10,30}$")
            .WithMessage(
                "Password must be between 10 and 30 characters, and include at least one digit, one lowercase letter, one uppercase letter, and one special character.")
            .NotEqual(x => x.OldPassword)
            .WithMessage("New password must be different from old password.");
    }
}