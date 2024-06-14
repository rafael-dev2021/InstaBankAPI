using AuthenticateAPI.Extensions;
using AuthenticateAPI.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace XUnitTests.AuthenticateAPI.Extensions;

public class UserRolesDataTests
{
    [Fact]
    public async Task AddUserRolesDataAsync_ShouldCallRoleAndUserMethods_WhenRepositoryIsNotNull()
    {
        // Arrange
        var userRoleRepositoryMock = new Mock<IUserRoleRepository>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var serviceScopeMock = new Mock<IServiceScope>();
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        var applicationBuilderMock = new Mock<IApplicationBuilder>();

        serviceProviderMock.Setup(sp => sp.GetService(typeof(IUserRoleRepository)))
            .Returns(userRoleRepositoryMock.Object);

        serviceScopeMock.Setup(s => s.ServiceProvider)
            .Returns(serviceProviderMock.Object);

        serviceScopeFactoryMock.Setup(sf => sf.CreateScope())
            .Returns(serviceScopeMock.Object);

        applicationBuilderMock.Setup(ab => ab.ApplicationServices.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactoryMock.Object);

        // Act
        await UserRolesData.AddUserRolesDataAsync(applicationBuilderMock.Object);

        // Assert
        userRoleRepositoryMock.Verify(r => r.RoleAsync(), Times.Once);
        userRoleRepositoryMock.Verify(r => r.UserAsync(), Times.Once);
    }

    [Fact]
    public async Task AddUserRolesDataAsync_ShouldNotThrow_WhenRepositoryIsNull()
    {
        // Arrange
        var serviceProviderMock = new Mock<IServiceProvider>();
        var serviceScopeMock = new Mock<IServiceScope>();
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        var applicationBuilderMock = new Mock<IApplicationBuilder>();

        serviceProviderMock.Setup(sp => sp.GetService(typeof(IUserRoleRepository)))
            .Returns(null!);

        serviceScopeMock.Setup(s => s.ServiceProvider)
            .Returns(serviceProviderMock.Object);

        serviceScopeFactoryMock.Setup(sf => sf.CreateScope())
            .Returns(serviceScopeMock.Object);

        applicationBuilderMock.Setup(ab => ab.ApplicationServices.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactoryMock.Object);

        // Act & Assert
        await UserRolesData.AddUserRolesDataAsync(applicationBuilderMock.Object);
    }
}