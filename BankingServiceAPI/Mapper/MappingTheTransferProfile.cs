using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Models;

namespace BankingServiceAPI.Mapper;

public class MappingTheTransferProfile : Profile
{
    public MappingTheTransferProfile()
    {
        CreateMap<Transfer, TransferByCpfDtoResponse>()
            .ConstructUsing(src => new TransferByCpfDtoResponse(
                src.Id,
                src.AccountDestination!.User!.Name!,
                src.AccountDestination.User.LastName!,
                src.AccountDestination.User.Cpf!,
                src.Amount,
                src.TransferDate
            ));

        CreateMap<Transfer, TransferByBankAccountNumberDtoResponse>()
            .ConstructUsing(src => new TransferByBankAccountNumberDtoResponse(
                src.Id,
                src.AccountDestination!.User!.Name!,
                src.AccountDestination.User.LastName!,
                src.AccountDestination.AccountNumber,
                src.Amount,
                src.TransferDate
            ));
    }
}