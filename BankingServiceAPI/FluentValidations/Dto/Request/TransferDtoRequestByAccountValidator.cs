using BankingServiceAPI.Dto.Request;
using FluentValidation;

namespace BankingServiceAPI.FluentValidations.Dto.Request;

public class TransferDtoRequestByAccountValidator : AbstractValidator<TransferDtoRequestByAccount>
{
    public TransferDtoRequestByAccountValidator()
    {
        RuleFor(x => x.OriginAccountNumber)
            .GreaterThan(0)
            .WithMessage("Origin account number must be greater than zero.");

        RuleFor(x => x.DestinationAccountNumber)
            .GreaterThan(0)
            .WithMessage("Destination account number must be greater than zero.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Transfer amount must be greater than zero.");
    }
}