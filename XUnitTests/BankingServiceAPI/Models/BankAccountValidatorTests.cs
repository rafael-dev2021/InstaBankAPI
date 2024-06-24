using BankingServiceAPI.FluentValidations.Models;
using BankingServiceAPI.Models;
using FluentValidation.TestHelper;

namespace XUnitTests.BankingServiceAPI.Models;

public class BankAccountValidatorTests
{
    private readonly BankAccountValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Id_Is_Less_Than_Or_Equal_To_Zero()
    {
        var model = new BankAccount();
        model.SetId(0);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id must be greater than zero.");

        model.SetId(-1);
        result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id must be greater than zero.");
    }

    [Fact]
    public void Should_Have_Error_When_AccountNumber_Is_Empty()
    {
        var model = new BankAccount();
        model.SetAccountNumber(0);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AccountNumber)
            .WithErrorMessage("Account number is required.");
    }

    [Fact]
    public void Should_Have_Error_When_AccountNumber_Is_Not_6_Digits()
    {
        var model = new BankAccount();
        model.SetAccountNumber(12345);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AccountNumber)
            .WithErrorMessage("Account number must be 6 digits long.");
    }

    [Fact]
    public void Should_Have_Error_When_Agency_Is_Empty()
    {
        var model = new BankAccount();
        model.SetAgency(0);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Agency)
            .WithErrorMessage("Agency number is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Agency_Is_Not_4_Digits()
    {
        var model = new BankAccount();
        model.SetAgency(123);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Agency)
            .WithErrorMessage("Agency number must be 4 digits long.");
    }

    [Fact]
    public void Should_Have_Error_When_Balance_Is_Negative()
    {
        var model = new BankAccount();
        model.SetBalance(-1);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Balance)
            .WithErrorMessage("Balance must be a positive number.");
    }

    [Fact]
    public void Should_Have_Error_When_AccountType_Is_Invalid()
    {
        var model = new BankAccount();
        model.SetAccountType((AccountType)999);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AccountType)
            .WithErrorMessage("Invalid account type.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var model = new BankAccount();
        model.SetId(1);
        model.SetAccountNumber(123456);
        model.SetAgency(1234);
        model.SetBalance(100);
        model.SetAccountType(AccountType.Savings);

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}