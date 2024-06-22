using AuthenticateAPI.Dto.Request;
using FluentValidation;

namespace AuthenticateAPI.FluentValidations.Dto.Request;

public class RefreshTokenDtoRequestValidator : AbstractValidator<RefreshTokenDtoRequest>
{
    public RefreshTokenDtoRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.");
    }
}