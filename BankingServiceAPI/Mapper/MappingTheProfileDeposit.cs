using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Models;

namespace BankingServiceAPI.Mapper;

public class MappingTheProfileDeposit : Profile
{
    public MappingTheProfileDeposit()
    {
        CreateMap<Deposit, DepositDtoResponse>()
            .ConstructUsing(src => new DepositDtoResponse(
                src.Id,
                src.AccountDestination!.User!.Name!,
                src.AccountDestination.User.LastName!,
                src.AccountDestination.User.Cpf!,
                src.AccountDestination.AccountNumber,
                src.Amount,
                src.TransferDate
            ));
    }
}