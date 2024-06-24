using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Mapper;
using BankingServiceAPI.Models;

namespace XUnitTests.BankingServiceAPI.Mapper;

public class MappingTheUserProfileTests
{
    private readonly IMapper _mapper;

    public MappingTheUserProfileTests()
    {
        var config = new MapperConfiguration(cfg => 
            cfg.AddProfile<MappingTheUserProfile>());
        
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Should_Map_User_To_UserDtoResponse()
    {
        // Arrange
        var user = new User();
        user.SetId("123");
        user.SetName("John");
        user.SetLastName("John");
        user.SetEmail("john.doe@example.com");
        user.SetPhoneNumber("123456789");
        user.SetCpf("123.456.789-00");
        user.SetRole("Admin");

        // Act
        var result = _mapper.Map<UserDtoResponse>(user);

        // Assert
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.LastName, result.LastName);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.PhoneNumber, result.PhoneNumber);
        Assert.Equal(user.Cpf, result.Cpf);
        Assert.Equal(user.Role, result.Role);
    }

    [Fact]
    public void Should_Map_UserDtoResponse_To_User()
    {
        // Arrange
        var userDto = new UserDtoResponse(
            "123", 
            "John", 
            "Doe", 
            "123.456.789-00",
            "john.doe@example.com",
            "123456789",
            "Admin");

        // Act
        var result = _mapper.Map<User>(userDto);

        // Assert
        Assert.Equal(userDto.Id, result.Id);
        Assert.Equal(userDto.Name, result.Name);
        Assert.Equal(userDto.LastName, result.LastName);
        Assert.Equal(userDto.Email, result.Email);
        Assert.Equal(userDto.Role, result.Role);
    }
}