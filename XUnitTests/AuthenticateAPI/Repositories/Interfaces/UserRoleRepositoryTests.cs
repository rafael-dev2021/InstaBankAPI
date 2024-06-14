using AuthenticateAPI.Repositories.Interfaces;
using FluentAssertions;
using Moq;

namespace XUnitTests.AuthenticateAPI.Repositories.Interfaces;

public class UserRoleRepositoryTests
{
    private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock = new();

    [Fact]
    public async Task UserAsync_Should_Be_Called_Once()
    {
        // Act
        await _userRoleRepositoryMock.Object.UserAsync();

        // Assert
        _userRoleRepositoryMock.Verify(repo => repo.UserAsync(), Times.Once);
    }

    [Fact]
    public async Task RoleAsync_Should_Be_Called_Once()
    {
        // Act
        await _userRoleRepositoryMock.Object.RoleAsync();

        // Assert
        _userRoleRepositoryMock.Verify(repo => repo.RoleAsync(), Times.Once);
    }

    [Fact]
    public async Task UserAsync_Should_Complete_Successfully()
    {
        // Arrange
        _userRoleRepositoryMock.Setup(repo => repo.UserAsync())
            .Returns(Task.CompletedTask);

        // Act
        var task = _userRoleRepositoryMock.Object.UserAsync();

        // Assert
        await task;
        task.IsCompletedSuccessfully.Should().BeTrue();
    }

    [Fact]
    public async Task RoleAsync_Should_Complete_Successfully()
    {
        // Arrange
        _userRoleRepositoryMock.Setup(repo => repo.RoleAsync())
            .Returns(Task.CompletedTask);

        // Act
        var task = _userRoleRepositoryMock.Object.RoleAsync();

        // Assert
        await task;
        task.IsCompletedSuccessfully.Should().BeTrue();
    }
}
