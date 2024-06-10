using AuthenticateAPI.Dto.Request;
using FluentValidation;

namespace FluentValidations.AuthenticateAPI.Dto.Request;

public class UpdateUserDtoRequestValidator : AbstractValidator<UpdateUserDtoRequest>
{
    public UpdateUserDtoRequestValidator()
    {
        RuleFor(x => x.Name)!.NameRules();
        RuleFor(x => x.LastName)!.LastNameRules();
        RuleFor(x => x.Email)!.EmailRules();
        RuleFor(x => x.PhoneNumber).PhoneNumberRules(x => x.PhoneNumber);
    }
}