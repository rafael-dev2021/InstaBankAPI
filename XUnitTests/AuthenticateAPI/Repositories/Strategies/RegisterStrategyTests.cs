using AuthenticateAPI.Context;
using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Strategies;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace XUnitTests.AuthenticateAPI.Repositories.Strategies;

public class RegisterStrategyTests : IDisposable
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly AppDbContext _appDbContext;
    private readonly RegisterStrategy _registerStrategy;

    public RegisterStrategyTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock =
            new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
        Mock<SignInManager<User>> signInManagerMock = new(_userManagerMock.Object, contextAccessorMock.Object,
            claimsFactoryMock.Object, null!, null!, null!, null!);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _appDbContext = new AppDbContext(options);

        _registerStrategy = new RegisterStrategy(signInManagerMock.Object, _userManagerMock.Object, _appDbContext);
    }

    [Fact(DisplayName = "Should return validation errors when CPF, email, and phone number are already used")]
    public async Task ValidateAsync_Should_ReturnValidationErrors()
    {
        // Arrange
        var existingUser = new User { Email = "john@example.com", PhoneNumber = "+1234567890" };
        existingUser.SetName("Test");
        existingUser.SetLastName("Test");
        existingUser.SetCpf("123.456.789-10");
        existingUser.SetRole("Admin");
        await _appDbContext.Users.AddAsync(existingUser);
        await _appDbContext.SaveChangesAsync();

        // Act
        var errors =
            await _registerStrategy.ValidateAsync(existingUser.Cpf, existingUser.Email, existingUser.PhoneNumber);

        // Assert
        errors.Should().Contain("CPF already used.");
        errors.Should().Contain("Email already used.");
        errors.Should().Contain("Phone number already used.");
    }

    [Fact(DisplayName = "Should return successful registration response")]
    public async Task CreateUserAsync_Should_Return_SuccessfulRegistrationResponse()
    {
        // Arrange
        var request = new RegisterDtoRequest(
            "John",
            "Doe",
            "+1234567890",
            "123.456.789-10",
            "john@example.com",
            "Admin",
            "123456789",
            "123456789"
        );

        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var response = await _registerStrategy.CreateUserAsync(request);

        // Assert
        response.Should().BeEquivalentTo(new RegisteredDtoResponse(true, "Registration successful."));
    }

    [Fact(DisplayName = "Should return error when passwords do not match")]
    public async Task CreateUserAsync_Should_Return_Error_When_Passwords_Do_Not_Match()
    {
        // Arrange
        var request = new RegisterDtoRequest(
            "John",
            "Doe",
            "+1234567890",
            "123.456.789-10",
            "john@example.com",
            "Admin",
            "123456789",
            "987654321"
        );

        // Act
        var response = await _registerStrategy.CreateUserAsync(request);

        // Assert
        response.Should()
            .BeEquivalentTo(new RegisteredDtoResponse(false, "Password and confirm password do not match."));
    }

    [Fact(DisplayName = "Should return error when validation fails")]
    public async Task CreateUserAsync_Should_Return_Error_When_Validation_Fails()
    {
        // Arrange
        var existingUser = new User { Email = "john@example.com", PhoneNumber = "+1234567890" };
        existingUser.SetName("Test");
        existingUser.SetLastName("Test");
        existingUser.SetCpf("123.456.789-10");
        existingUser.SetRole("Admin");
        await _appDbContext.Users.AddAsync(existingUser);
        await _appDbContext.SaveChangesAsync();

        var request = new RegisterDtoRequest(
            "John",
            "Doe",
            "+1234567890",
            "123.456.789-10",
            "john@example.com",
            "Admin",
            "123456789",
            "123456789"
        );

        // Act
        var response = await _registerStrategy.CreateUserAsync(request);

        // Assert
        response.Should().BeEquivalentTo(new RegisteredDtoResponse(false,
            "CPF already used., Email already used., Phone number already used."));
    }

    [Fact(DisplayName = "Should return error when user creation fails")]
    public async Task CreateUserAsync_Should_Return_Error_When_User_Creation_Fails()
    {
        // Arrange
        var request = new RegisterDtoRequest(
            "John",
            "Doe",
            "+1234567890",
            "123.456.789-10",
            "john@example.com",
            "Admin",
            "123456789",
            "123456789"
        );

        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());

        // Act
        var response = await _registerStrategy.CreateUserAsync(request);

        // Assert
        response.Should().BeEquivalentTo(new RegisteredDtoResponse(false, "Registration failed."));
    }

    [Fact(DisplayName = "Should return password mismatch error")]
    public async Task CreateUserAsync_Should_Return_Error_When_Passwords_DoNotMatch()
    {
        // Arrange
        var request = new RegisterDtoRequest(
            "John",
            "Doe",
            "+1234567890",
            "123.456.789-10",
            "john@example.com",
            "Admin",
            "Password123!",
            "Password456!"
        );

        // Act
        var result = await _registerStrategy.CreateUserAsync(request);

        // Assert
        result.Should().BeEquivalentTo(new RegisteredDtoResponse(false, "Password and confirm password do not match."));
    }

    public void Dispose()
    {
        _appDbContext.Database.EnsureDeleted();
        _appDbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}