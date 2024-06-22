using AuthenticateAPI.Models;
using FluentValidation;

namespace AuthenticateAPI.FluentValidations.Models;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Name)!.NameRules();
        RuleFor(x => x.LastName)!.LastNameRules();
        RuleFor(x => x.Email)!.EmailRules();
        RuleFor(x => x.PhoneNumber).PhoneNumberRules(x => x.PhoneNumber);
        RuleFor(x => x.Cpf).CpfRules();
        RuleFor(x => x.Tokens)
            .Must(tokens => tokens != null && !tokens.Contains(null!))
            .WithMessage("Tokens collection cannot contain null elements."); 
    }
}