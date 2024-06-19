using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AutoMapper;

namespace AuthenticateAPI.Mapper;

public class MappingTheUserProfile : Profile
{
    public MappingTheUserProfile()
    {
        CreateMap<User, UserDtoResponse>().ReverseMap();
    }
}