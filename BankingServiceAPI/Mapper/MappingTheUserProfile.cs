using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Models;

namespace BankingServiceAPI.Mapper;

public class MappingTheUserProfile : Profile
{
    public MappingTheUserProfile()
    {
        CreateMap<User, UserDtoResponse>().ReverseMap();
    }
}