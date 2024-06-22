using AuthenticateAPI.Context;
using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories;
using AuthenticateAPI.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace XUnitTests.AuthenticateAPI.Repositories;

public class AuthenticatedRepositoryTests
{
    private readonly Mock<IAuthenticatedStrategy> _authenticatedStrategyMock;
    private readonly AuthenticatedRepository _authenticatedRepository;
    private readonly Mock<IRegisterStrategy> _registerStrategyMock;
    private readonly Mock<IUpdateProfileStrategy> _updateProfileStrategyMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly AppDbContext _appDbContext;

    protected AuthenticatedRepositoryTests()
    {
           var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);

        _registerStrategyMock = new Mock<IRegisterStrategy>();
        _authenticatedStrategyMock = new Mock<IAuthenticatedStrategy>();
        _updateProfileStrategyMock = new Mock<IUpdateProfileStrategy>();
        _signInManagerMock = new Mock<SignInManager<User>>();

        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock =
            new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
        _signInManagerMock = new Mock<SignInManager<User>>(
            _userManagerMock.Object,
            contextAccessorMock.Object,
            userPrincipalFactoryMock.Object,
            null!, null!, null!, null!
        );

        _authenticatedRepository = new AuthenticatedRepository(
            _signInManagerMock.Object,
            _userManagerMock.Object,
            _authenticatedStrategyMock.Object,
            _registerStrategyMock.Object,
            _updateProfileStrategyMock.Object,
            _appDbContext
        );
    }

    public class GetAllUsersAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "GetAllUsersAsync should return users with roles")]
        public async Task GetAllUsersAsync_Should_Return_Users_With_Roles()
        {
            // Arrange
            var user1 = new User { Id = "1", Email = "user1@example.com", PhoneNumber = "+5540028922" };
            user1.SetName("Name 1");
            user1.SetLastName("Last Name 1");
            user1.SetCpf("123.456.789-10");
            user1.SetRole("Admin");

            var user2 = new User { Id = "2", Email = "user2@example.com", PhoneNumber = "+5540028921" };
            user2.SetName("Name 2");
            user2.SetLastName("Last Name 2");
            user2.SetCpf("123.456.789-11"); 
            user2.SetRole("User"); 

            _appDbContext.Users.AddRange(user1, user2);
            await _appDbContext.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string> { "Admin" });

            // Act
            var result = await _authenticatedRepository.GetAllUsersAsync();

            // Assert
            var enumerable = result as User[] ?? result.ToArray();
            enumerable.Should().HaveCount(2);
            enumerable.First().Role.Should().Be("Admin");
            _userManagerMock.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "GetAllUsersAsync should handle empty user list")]
        public async Task GetAllUsersAsync_Should_Handle_Empty_User_List()
        {
            // Act
            var result = await _authenticatedRepository.GetAllUsersAsync();

            // Assert
            result.Should().BeEmpty();
            _userManagerMock.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Never);
        }
    }

    public class AuthenticateAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "AuthenticateAsync should return success response when login is successful")]
        public async Task AuthenticateAsync_Should_Return_Success_Response()
        {
            // Arrange
            var request = new LoginDtoRequest("john@example.com", "password123", true);
            var expectedResponse = new AuthenticatedDtoResponse(true, "Login successful.");

            _authenticatedStrategyMock
                .Setup(s => s.AuthenticatedAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _authenticatedRepository.AuthenticateAsync(request);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _authenticatedStrategyMock.Verify(s => s.AuthenticatedAsync(request), Times.Once);
        }

        [Fact(DisplayName = "AuthenticateAsync should return error response when login fails")]
        public async Task AuthenticateAsync_Should_Return_Error_Response()
        {
            // Arrange
            var request = new LoginDtoRequest("john@example.com", "wrongpassword", true);
            var expectedResponse = new AuthenticatedDtoResponse(false, "Invalid email or password. Please try again.");

            _authenticatedStrategyMock
                .Setup(s => s.AuthenticatedAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _authenticatedRepository.AuthenticateAsync(request);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _authenticatedStrategyMock.Verify(s => s.AuthenticatedAsync(request), Times.Once);
        }
    }

    public class RegisterAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "RegisterAsync should return success response when registration is successful")]
        public async Task RegisterAsync_Should_Return_Success_Response()
        {
            // Arrange
            var request = new RegisterDtoRequest("John", "Doe", "+1234567890", "123.456.789-01", "john.doe@example.com",
                "Admin","StrongP@ssw0rd", "StrongP@ssw0rd");

            var expectedResponse = new RegisteredDtoResponse(true, "Registration successful.");

            _registerStrategyMock
                .Setup(s => s.ValidateAsync(request.Cpf, request.Email, request.PhoneNumber))
                .ReturnsAsync([]);

            _registerStrategyMock
                .Setup(s => s.CreateUserAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _authenticatedRepository.RegisterAsync(request);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _registerStrategyMock.Verify(s => s.ValidateAsync(request.Cpf, request.Email, request.PhoneNumber),
                Times.Once);
            _registerStrategyMock.Verify(s => s.CreateUserAsync(request), Times.Once);
        }

        [Fact(DisplayName = "RegisterAsync should return error response when validation fails")]
        public async Task RegisterAsync_Should_Return_Error_Response()
        {
            // Arrange
            var request = new RegisterDtoRequest("John", "Doe", "+1234567890", "123.456.789-01", "john.doe@example.com",
                "Admin", "StrongP@ssw0rd", "StrongP@ssw0r");

            var validationErrors = new List<string> { "Email already used." };
            var expectedResponse = new RegisteredDtoResponse(false, string.Join(Environment.NewLine, validationErrors));

            _registerStrategyMock
                .Setup(s => s.ValidateAsync(request.Cpf, request.Email, request.PhoneNumber))
                .ReturnsAsync(validationErrors);

            // Act
            var response = await _authenticatedRepository.RegisterAsync(request);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _registerStrategyMock.Verify(s => s.ValidateAsync(request.Cpf, request.Email, request.PhoneNumber),
                Times.Once);
            _registerStrategyMock.Verify(s => s.CreateUserAsync(It.IsAny<RegisterDtoRequest>()), Times.Never);
        }
    }

    public class UpdateProfileAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "UpdateProfileAsync should return success response when profile update is successful")]
        public async Task UpdateProfileAsync_Should_Return_Success_Response()
        {
            // Arrange
            const string userId = "12345";
            var request = new UpdateUserDtoRequest("NewName", "NewLastName", "new.email@example.com", "+1234567890");
            var expectedResponse = new UpdatedDtoResponse(true, "Profile updated successfully.");

            _updateProfileStrategyMock
                .Setup(s => s.UpdateProfileAsync(request, userId))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _authenticatedRepository.UpdateProfileAsync(request, userId);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _updateProfileStrategyMock.Verify(s => s.UpdateProfileAsync(request, userId), Times.Once);
        }

        [Fact(DisplayName = "UpdateProfileAsync should return error response when profile update fails")]
        public async Task UpdateProfileAsync_Should_Return_Error_Response()
        {
            // Arrange
            const string userId = "12345";
            var request = new UpdateUserDtoRequest("NewName", "NewLastName", "new.email@example.com", "+1234567890");
            var validationErrors = new List<string> { "Email already used by another user." };
            var expectedResponse = new UpdatedDtoResponse(false, string.Join(Environment.NewLine, validationErrors));

            _updateProfileStrategyMock
                .Setup(s => s.UpdateProfileAsync(request, userId))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _authenticatedRepository.UpdateProfileAsync(request, userId);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _updateProfileStrategyMock.Verify(s => s.UpdateProfileAsync(request, userId), Times.Once);
        }
    }

    public class ChangePasswordAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "ChangePasswordAsync should return true when password change is successful")]
        public async Task ChangePasswordAsync_Should_Return_True_When_Password_Change_Is_Successful()
        {
            // Arrange
            var request = new ChangePasswordDtoRequest(
                "john@example.com",
                "OldP@ssword",
                "NewP@ssword123");

            var user = new User { Email = "john@example.com" };

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(request.Email!))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(um => um.ChangePasswordAsync(user, request.CurrentPassword!, request.NewPassword!))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authenticatedRepository.ChangePasswordAsync(request);

            // Assert
            result.Should().BeTrue();
            _userManagerMock.Verify(um => um.FindByEmailAsync(request.Email!), Times.Once);
            _userManagerMock.Verify(um => um.ChangePasswordAsync(user, request.CurrentPassword!, request.NewPassword!),
                Times.Once);
        }

        [Fact(DisplayName = "ChangePasswordAsync should return false when user is not found")]
        public async Task ChangePasswordAsync_Should_Return_False_When_User_Not_Found()
        {
            // Arrange
            var request = new ChangePasswordDtoRequest(
                "nonexistent@example.com",
                "OldP@ssword",
                "NewP@ssword123");

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(request.Email!))
                .ReturnsAsync((User)null!);

            // Act
            var result = await _authenticatedRepository.ChangePasswordAsync(request);

            // Assert
            result.Should().BeFalse();
            _userManagerMock.Verify(um => um.FindByEmailAsync(request.Email!), Times.Once);
            _userManagerMock.Verify(
                um => um.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact(DisplayName = "ChangePasswordAsync should return false when password change fails")]
        public async Task ChangePasswordAsync_Should_Return_False_When_Password_Change_Fails()
        {
            // Arrange
            var request = new ChangePasswordDtoRequest(
                "john@example.com",
                "OldP@ssword",
                "NewP@ssword123");
            var user = new User { Email = "john@example.com" };

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(request.Email!))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(um => um.ChangePasswordAsync(user, request.CurrentPassword!, request.NewPassword!))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password change failed." }));

            // Act
            var result = await _authenticatedRepository.ChangePasswordAsync(request);

            // Assert
            result.Should().BeFalse();
            _userManagerMock.Verify(um => um.FindByEmailAsync(request.Email!), Times.Once);
            _userManagerMock.Verify(um => um.ChangePasswordAsync(user, request.CurrentPassword!, request.NewPassword!),
                Times.Once);
        }
    }

    public class GetUserProfileAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "GetUserProfileAsync should return user profile when user is found")]
        public async Task GetUserProfileAsync_Should_Return_User_Profile_When_User_Is_Found()
        {
            // Arrange
            const string userEmail = "john@example.com";
            var user = new User
            {
                Email = userEmail,
                PhoneNumber = "+1234567890"
            };
            user.SetName("John");
            user.SetLastName("Doe");

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(userEmail))
                .ReturnsAsync(user);

            // Act
            var result = await _authenticatedRepository.GetUserProfileAsync(userEmail);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be(user.Email);
            result.Name.Should().Be(user.Name);
            result.LastName.Should().Be(user.LastName);
            result.PhoneNumber.Should().Be(user.PhoneNumber);
            _userManagerMock.Verify(um => um.FindByEmailAsync(userEmail), Times.Once);
        }

        [Fact(DisplayName = "GetUserProfileAsync should return null when user is not found")]
        public async Task GetUserProfileAsync_Should_Return_Null_When_User_Is_Not_Found()
        {
            // Arrange
            const string userEmail = "nonexistent@example.com";

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(userEmail))
                .ReturnsAsync((User)null!);

            // Act
            var result = await _authenticatedRepository.GetUserProfileAsync(userEmail);

            // Assert
            result.Should().BeNull();
            _userManagerMock.Verify(um => um.FindByEmailAsync(userEmail), Times.Once);
        }
    }

    public class ForgotPasswordAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "ForgotPasswordAsync should return true when password reset is successful")]
        public async Task ForgotPasswordAsync_Should_Return_True_When_Password_Reset_Is_Successful()
        {
            // Arrange
            const string userEmail = "john@example.com";
            const string newPassword = "NewP@ssword123";

            var user = new User { Email = userEmail };

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(userEmail))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(um => um.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("token");

            _userManagerMock
                .Setup(um => um.ResetPasswordAsync(user, "token", newPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authenticatedRepository.ForgotPasswordAsync(userEmail, newPassword);

            // Assert
            result.Should().BeTrue();
            _userManagerMock.Verify(um => um.FindByEmailAsync(userEmail), Times.Once);
            _userManagerMock.Verify(um => um.GeneratePasswordResetTokenAsync(user), Times.Once);
            _userManagerMock.Verify(um => um.ResetPasswordAsync(user, "token", newPassword), Times.Once);
        }

        [Fact(DisplayName = "ForgotPasswordAsync should return false when user is not found")]
        public async Task ForgotPasswordAsync_Should_Return_False_When_User_Not_Found()
        {
            // Arrange
            const string userEmail = "nonexistent@example.com";
            const string newPassword = "NewP@ssword123";

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(userEmail))
                .ReturnsAsync((User)null!);

            // Act
            var result = await _authenticatedRepository.ForgotPasswordAsync(userEmail, newPassword);

            // Assert
            result.Should().BeFalse();
            _userManagerMock.Verify(um => um.FindByEmailAsync(userEmail), Times.Once);
            _userManagerMock.Verify(
                um => um.GeneratePasswordResetTokenAsync(It.IsAny<User>()),
                Times.Never);
            _userManagerMock.Verify(
                um => um.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }
    }

    public class LogoutAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "LogoutAsync should sign out successfully")]
        public async Task LogoutAsync_Should_Sign_Out_Successfully()
        {
            // Act
            await _authenticatedRepository.LogoutAsync();

            // Assert
            _signInManagerMock.Verify(sm => sm.SignOutAsync(), Times.Once);
        }
    }
    
    public class SaveAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "SaveAsync should update user in the database")]
        public async Task SaveAsync_Should_Update_User_In_Database()
        {
            // Arrange
            var user = new User { Id = "1", Email = "user1@example.com", PhoneNumber = "+5540028922" };
            user.SetName("Name 1");
            user.SetLastName("Last Name 1");
            user.SetCpf("123.456.789-10");
            user.SetRole("Admin");

            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            // Update user details
            user.SetName("Updated Name 1");
            user.SetLastName("Updated Last Name 1");

            // Act
            await _authenticatedRepository.SaveAsync(user);

            // Assert
            var updatedUser = await _appDbContext.Users.FindAsync(user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser!.Name.Should().Be("Updated Name 1");
            updatedUser.LastName.Should().Be("Updated Last Name 1");
        }

        [Fact(DisplayName = "SaveAsync should throw an exception if user is null")]
        public async Task SaveAsync_Should_Throw_Exception_If_User_Is_Null()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _authenticatedRepository.SaveAsync(null!));
        }
    }
}