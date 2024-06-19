using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Mapper;
using AuthenticateAPI.Models;
using AutoMapper;

namespace XUnitTests.AuthenticateAPI.Mapper;

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
        var user = new User
        {
            Id = "123",
            Email = "john.doe@example.com"
        };
        user.SetName("John");
        user.SetLastName("John");
        user.SetRole("Admin");

        // Act
        var result = _mapper.Map<UserDtoResponse>(user);

        // Assert
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.LastName, result.LastName);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Role, result.Role);
    }

    [Fact]
    public void Should_Map_UserDtoResponse_To_User()
    {
        // Arrange
        var userDto = new UserDtoResponse("123", "John", "Doe", "john.doe@example.com", "Admin");

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