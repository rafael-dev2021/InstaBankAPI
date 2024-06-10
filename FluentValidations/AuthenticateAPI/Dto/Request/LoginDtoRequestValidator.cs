﻿using AuthenticateAPI.Dto.Request;
using FluentValidation;

namespace FluentValidations.AuthenticateAPI.Dto.Request;

public class LoginDtoRequestValidator : AbstractValidator<LoginDtoRequest>
{
    public LoginDtoRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
        
        RuleFor(x => x.RememberMe)
            .Must(value => true)
            .WithMessage("RememberMe must be a boolean value.");
    }
}