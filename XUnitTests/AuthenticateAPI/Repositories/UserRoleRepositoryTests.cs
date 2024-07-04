using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace XUnitTests.AuthenticateAPI.Repositories;

public class UserRoleRepositoryTests
{
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly UserRoleRepository _userRoleRepository;

    protected UserRoleRepositoryTests()
    {
        var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            roleStoreMock.Object, null!, null!, null!, null!);

        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _userRoleRepository = new UserRoleRepository(_roleManagerMock.Object, _userManagerMock.Object);
    }

    public class CreateRoleIfNotExistsAsyncTests : UserRoleRepositoryTests
    {
        [Fact(DisplayName = "CreateRoleIfNotExistsAsync should create role if it does not exist")]
        public async Task CreateRoleIfNotExistsAsync_Should_Create_Role_If_Not_Exists()
        {
            // Arrange
            const string roleName = "Admin";
            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(roleName))
                .ReturnsAsync(false);

            _roleManagerMock.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _userRoleRepository.CreateRoleIfNotExistsAsync(roleName);

            // Assert
            _roleManagerMock.Verify(rm => rm.RoleExistsAsync(roleName), Times.Once);
            _roleManagerMock.Verify(
                rm => rm.CreateAsync(It.Is<IdentityRole>(role =>
                    role.Name == roleName && role.NormalizedName!.Equals(roleName, StringComparison.CurrentCultureIgnoreCase))), Times.Once);
        }

        [Fact(DisplayName = "CreateRoleIfNotExistsAsync should not create role if it exists")]
        public async Task CreateRoleIfNotExistsAsync_Should_Not_Create_Role_If_Exists()
        {
            // Arrange
            const string roleName = "Admin";
            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(roleName))
                .ReturnsAsync(true);

            // Act
            await _userRoleRepository.CreateRoleIfNotExistsAsync(roleName);

            // Assert
            _roleManagerMock.Verify(rm => rm.RoleExistsAsync(roleName), Times.Once);
            _roleManagerMock.Verify(rm => rm.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
        }
    }

    public class CreateUserIfNotExistsAsyncTests : UserRoleRepositoryTests
    {
        [Fact(DisplayName = "CreateUserIfNotExistsAsync should create user if not exists and add to role")]
        public async Task CreateUserIfNotExistsAsync_Should_Create_User_If_Not_Exists_And_Add_To_Role()
        {
            // Arrange
            const string email = "rafael@example.com";
            const string roleName = "User";

            _userManagerMock.Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync((User)null!);

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), roleName))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _userRoleRepository.CreateUserIfNotExistsAsync(email, roleName,"Test","Test","123.446.676-44","+5540028921");

            // Assert
            _userManagerMock.Verify(um => um.FindByEmailAsync(email), Times.Once);
            _userManagerMock.Verify(um => um.CreateAsync(It.Is<User>(user =>
                user.Email == email &&
                user.UserName == email &&
                user.NormalizedEmail!.Equals(email, StringComparison.CurrentCultureIgnoreCase) &&
                user.NormalizedUserName!.Equals(email, StringComparison.CurrentCultureIgnoreCase) &&
                user.PhoneNumber == "+5540028921" &&
                user.PhoneNumberConfirmed &&
                user.EmailConfirmed), "@Visual24k+"), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), roleName), Times.Once);
        }

        [Fact(DisplayName = "CreateUserIfNotExistsAsync should not create user if exists")]
        public async Task CreateUserIfNotExistsAsync_Should_Not_Create_User_If_Exists()
        {
            // Arrange
            const string email = "rafael@example.com";
            const string roleName = "User";
            var existingUser = new User { Email = email };

            _userManagerMock.Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync(existingUser);

            // Act
            await _userRoleRepository.CreateUserIfNotExistsAsync(email, roleName,"Test","Test","123.446.676-24","+5540028922");

            // Assert
            _userManagerMock.Verify(um => um.FindByEmailAsync(email), Times.Once);
            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }
    }

    public class UserAsyncTests : UserRoleRepositoryTests
    {
        [Fact(DisplayName = "UserAsync should create admin user if not exists")]
        public async Task UserAsync_Should_Create_Admin_User_If_Not_Exists()
        {
            // Arrange
            const string adminEmail = "admin@localhost.com";
            const string roleName = "Admin";

            _userManagerMock.Setup(um => um.FindByEmailAsync(adminEmail))
                .ReturnsAsync((User)null!);

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), roleName))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _userRoleRepository.UserAsync();

            // Assert
            _userManagerMock.Verify(um => um.FindByEmailAsync(adminEmail), Times.Once);
            _userManagerMock.Verify(um => um.CreateAsync(It.Is<User>(user =>
                user.Email == adminEmail &&
                user.UserName == adminEmail &&
                user.NormalizedEmail!.Equals(adminEmail, StringComparison.CurrentCultureIgnoreCase) &&
                user.NormalizedUserName!.Equals(adminEmail, StringComparison.CurrentCultureIgnoreCase) &&
                user.PhoneNumber == "+5540028922" &&
                user.PhoneNumberConfirmed &&
                user.EmailConfirmed), "@Visual24k+"), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), roleName), Times.Once);
        }

        [Fact(DisplayName = "UserAsync should create user if not exists")]
        public async Task UserAsync_Should_Create_User_If_Not_Exists()
        {
            // Arrange
            const string userEmail = "user@localhost.com";
            const string roleName = "User";

            _userManagerMock.Setup(um => um.FindByEmailAsync(userEmail))
                .ReturnsAsync((User)null!);

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), roleName))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _userRoleRepository.UserAsync();

            // Assert
            _userManagerMock.Verify(um => um.FindByEmailAsync(userEmail), Times.Once);
            _userManagerMock.Verify(um => um.CreateAsync(It.Is<User>(user =>
                user.Email == userEmail &&
                user.UserName == userEmail &&
                user.NormalizedEmail!.Equals(userEmail, StringComparison.CurrentCultureIgnoreCase) &&
                user.NormalizedUserName!.Equals(userEmail, StringComparison.CurrentCultureIgnoreCase) &&
                user.PhoneNumber == "+5540028923" &&
                user.PhoneNumberConfirmed &&
                user.EmailConfirmed), "@Visual24k+"), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), roleName), Times.Once);
        }
    }

    public class RoleAsyncTests : UserRoleRepositoryTests
    {
        [Fact(DisplayName = "RoleAsync should create admin role if not exists")]
        public async Task RoleAsync_Should_Create_Admin_Role_If_Not_Exists()
        {
            // Arrange
            const string roleName = "Admin";
            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(roleName))
                .ReturnsAsync(false);

            _roleManagerMock.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _userRoleRepository.RoleAsync();

            // Assert
            _roleManagerMock.Verify(rm => rm.RoleExistsAsync(roleName), Times.Once);
            _roleManagerMock.Verify(
                rm => rm.CreateAsync(It.Is<IdentityRole>(role =>
                    role.Name == roleName && role.NormalizedName!.Equals(roleName, StringComparison.CurrentCultureIgnoreCase))), Times.Once);
        }

        [Fact(DisplayName = "RoleAsync should create user role if not exists")]
        public async Task RoleAsync_Should_Create_User_Role_If_Not_Exists()
        {
            // Arrange
            const string roleName = "User";
            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(roleName))
                .ReturnsAsync(false);

            _roleManagerMock.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _userRoleRepository.RoleAsync();

            // Assert
            _roleManagerMock.Verify(rm => rm.RoleExistsAsync(roleName), Times.Once);
            _roleManagerMock.Verify(
                rm => rm.CreateAsync(It.Is<IdentityRole>(role =>
                    role.Name == roleName && role.NormalizedName!.Equals(roleName, StringComparison.CurrentCultureIgnoreCase))), Times.Once);
        }
    }
}