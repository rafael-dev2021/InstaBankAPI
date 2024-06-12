using AuthenticateAPI.Dto.Request;
using FluentValidation;

namespace AuthenticateAPI.FluentValidations.Dto.Request;

public class RegisterDtoRequestValidator : AbstractValidator<RegisterDtoRequest>
{
    public RegisterDtoRequestValidator()
    {
        RuleFor(x => x.Name)!.NameRules();
        RuleFor(x => x.LastName)!.LastNameRules();
        RuleFor(x => x.PhoneNumber).PhoneNumberRules(x => x.PhoneNumber);
        RuleFor(x => x.Cpf).CpfRules();
        RuleFor(x => x.Email)!.EmailRules();
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Matches(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=!])(?=\S+).{10,30}$")
            .WithMessage("Password must be strong and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")
            .Length(10, 30).WithMessage("Password must be between 10 and 30 characters.");
        
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}