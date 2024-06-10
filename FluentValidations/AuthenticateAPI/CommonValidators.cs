using System.Linq.Expressions;
using FluentValidation;

namespace FluentValidations.AuthenticateAPI;

public static class CommonValidators
{
    public static void NameRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        ruleBuilder
            .NotEmpty()
            .WithMessage("Name is required.")
            .Length(2, 50)
            .WithMessage("Name must be between 2 and 50 characters long.");
    }

    public static void LastNameRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        ruleBuilder
            .NotEmpty()
            .WithMessage("Last name is required.")
            .Length(2, 50)
            .WithMessage("Last name must be between 2 and 50 characters long.");
    }

    public static void EmailRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        ruleBuilder
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");
    }

    public static void PhoneNumberRules<T>(this IRuleBuilder<T, string?> ruleBuilder, Expression<Func<T, string?>> propertyExpression)
    {
        ruleBuilder
            .Matches(@"^\+?[0-9]{1,15}$")
            .WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrWhiteSpace(propertyExpression.Compile()(x)));
    }

    public static void CpfRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        ruleBuilder
            .Matches(@"^\d{3}\.\d{3}\.\d{3}\-\d{2}$")
            .WithMessage("Invalid CPF format.");
    }
}
