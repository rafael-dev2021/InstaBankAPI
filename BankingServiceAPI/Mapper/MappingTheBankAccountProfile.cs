using AutoMapper;
using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Models;

namespace BankingServiceAPI.Mapper;

public class MappingTheBankAccountProfile : Profile
{
    public MappingTheBankAccountProfile()
    {
        CreateMap<BankAccount, BankAccountDtoResponse>()
            .ForMember(dest => dest.UserDtoResponse, opt => opt.MapFrom(src => src.User))
            .ConstructUsing((src, context) => new BankAccountDtoResponse(
                src.Id,
                src.AccountNumber,
                src.Balance,
                src.Agency,
                src.AccountType.ToString(),
                context.Mapper.Map<UserDtoResponse>(src.User) 
            ));

        CreateMap<BankAccount, BankAccountDtoRequest>().ReverseMap();
    }
}