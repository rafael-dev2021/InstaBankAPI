using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Models;

namespace BankingServiceAPI.Mapper;

public class MappingTheWithdrawProfile : Profile
{
    public MappingTheWithdrawProfile()
    {
        CreateMap<Withdraw, WithdrawDtoResponse>()
            .ConstructUsing(src => new WithdrawDtoResponse(
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