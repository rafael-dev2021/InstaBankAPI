using AuthenticateAPI.Dto.Response;
using FluentValidation;

namespace AuthenticateAPI.FluentValidations.Dto.Response;

public class TokenDtoResponseValidator : AbstractValidator<TokenDtoResponse>
{
    public TokenDtoResponseValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.")
            .MinimumLength(10).WithMessage("Token must be at least 10 characters long.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .MinimumLength(10).WithMessage("Refresh token must be at least 10 characters long.");
    }
}