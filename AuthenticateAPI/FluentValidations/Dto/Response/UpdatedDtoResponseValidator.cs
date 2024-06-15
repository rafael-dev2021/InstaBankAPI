using AuthenticateAPI.Dto.Response;
using FluentValidation;

namespace AuthenticateAPI.FluentValidations.Dto.Response;

public class UpdatedDtoResponseValidator : AbstractValidator<UpdatedDtoResponse>
{
    public UpdatedDtoResponseValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Update failed.")
            .When(x => !x.Success);

        RuleFor(x => x.Message)
            .Empty().WithMessage("Update successful.")
            .When(x => x.Success);
    }
}