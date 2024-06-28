using BankingServiceAPI.Dto.Request;
using FluentValidation;

namespace BankingServiceAPI.FluentValidations.Dto.Request;

public class TransferDtoRequestByCpfValidator : AbstractValidator<TransferDtoRequestByCpf>
{
    public TransferDtoRequestByCpfValidator()
    {
        RuleFor(x => x.OriginCpf)
            .NotEmpty()
            .WithMessage("Origin CPF cannot be empty.")
            .Matches(@"^\d{3}\.\d{3}\.\d{3}\-\d{2}$")
            .WithMessage("Invalid CPF format.");

        RuleFor(x => x.DestinationCpf)
            .NotEmpty()
            .WithMessage("Destination CPF cannot be empty.")
            .Matches(@"^\d{3}\.\d{3}\.\d{3}\-\d{2}$")
            .WithMessage("Invalid CPF format.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Transfer amount must be greater than zero.");
    }
}