﻿using AuthenticateAPI.Context;
using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Strategies;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;

namespace XUnitTests.AuthenticateAPI.Repositories.Strategies;

public class UpdateProfileStrategyTests : IDisposable
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly AppDbContext _appDbContext;
    private readonly UpdateProfileStrategy _updateProfileStrategy;

    public UpdateProfileStrategyTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock =
            new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _appDbContext = new AppDbContext(options);

        _updateProfileStrategy = new UpdateProfileStrategy(_userManagerMock.Object, _appDbContext);
    }

    [Fact(DisplayName = "UpdateProfileAsync should return success response when profile update is successful")]
    public async Task UpdateProfileAsync_Should_Return_Success_Response()
    {
        // Arrange
        const string userId = "12345";
        var request = new UpdateUserDtoRequest("NewName", "NewLastName", "new.email@example.com", "+1234567890");
        var existingUser = new User { Id = userId, Email = "old.email@example.com", PhoneNumber = "+9876543210" };
        existingUser.SetName("OldName");
        existingUser.SetLastName("OldLastName");
        existingUser.SetRole("Admin");
        
        await _appDbContext.Users.AddAsync(existingUser);
        await _appDbContext.SaveChangesAsync();

        _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(existingUser);
        _userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

        // Act
        var response = await _updateProfileStrategy.UpdateProfileAsync(request, userId);

        // Assert
        response.Should().BeEquivalentTo(new UpdatedDtoResponse(true, "Profile updated successfully."));
        _userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact(DisplayName = "UpdateProfileAsync should return error response when validation fails")]
    public async Task UpdateProfileAsync_Should_Return_Error_Response()
    {
        // Arrange
        const string userId = "12345";
        var request = new UpdateUserDtoRequest("NewName", "NewLastName", "existing.email@example.com", "+9876543210");

        var existingUser = new User { Id = userId, Email = "old.email@example.com", PhoneNumber = "+1234567890" };
        existingUser.SetName("ExistingName");
        existingUser.SetLastName("ExistingLastName");
        existingUser.SetRole("Admin");
        
        var anotherUser = new User { Id = "54321", Email = "existing.email@example.com", PhoneNumber = "+9876543210" };
        anotherUser.SetName("AnotherName");
        anotherUser.SetLastName("AnotherLastName");
        anotherUser.SetRole("Admin");
        
        await _appDbContext.Users.AddAsync(existingUser);
        await _appDbContext.Users.AddAsync(anotherUser);
        await _appDbContext.SaveChangesAsync();

        _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(existingUser);

        // Act
        var response = await _updateProfileStrategy.UpdateProfileAsync(request, userId);

        // Assert
        response.Success.Should().BeFalse();
        response.Message.Should().Contain("Email already used by another user.");
        response.Message.Should().Contain("Phone number already used by another user.");
        _userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact(DisplayName = "UpdateProfileAsync should update Name and LastName when provided")]
    public async Task UpdateProfileAsync_Should_Update_Name_And_LastName()
    {
        // Arrange
        const string userId = "12345";
        var request = new UpdateUserDtoRequest("NewName", "NewLastName", "old.email@example.com", "+1234567890");

        var existingUser = new User { Id = userId, Email = "old.email@example.com", PhoneNumber = "+1234567890" };
        existingUser.SetName("OldName");
        existingUser.SetLastName("OldLastName");
        existingUser.SetRole("Admin");
        
        await _appDbContext.Users.AddAsync(existingUser);
        await _appDbContext.SaveChangesAsync();

        _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(existingUser);
        _userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

        // Act
        var response = await _updateProfileStrategy.UpdateProfileAsync(request, userId);

        // Assert
        response.Should().BeEquivalentTo(new UpdatedDtoResponse(true, "Profile updated successfully."));
        _userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<User>()), Times.Once);
        existingUser.Name.Should().Be("NewName");
        existingUser.LastName.Should().Be("NewLastName");
    }

    [Fact(DisplayName = "UpdateProfileAsync should not update Name and LastName if not provided")]
    public async Task UpdateProfileAsync_Should_Not_Update_Name_And_LastName_If_Not_Provided()
    {
        // Arrange
        const string userId = "12345";
        var request = new UpdateUserDtoRequest(null, null, "old.email@example.com", "+1234567890");

        var existingUser = new User { Id = userId, Email = "old.email@example.com", PhoneNumber = "+1234567890" };
        existingUser.SetName("OldName");
        existingUser.SetLastName("OldLastName");
        existingUser.SetRole("user");
        
        await _appDbContext.Users.AddAsync(existingUser);
        await _appDbContext.SaveChangesAsync();

        _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(existingUser);
        _userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

        // Act
        var response = await _updateProfileStrategy.UpdateProfileAsync(request, userId);

        // Assert
        response.Should().BeEquivalentTo(new UpdatedDtoResponse(true, "Profile updated successfully."));
        _userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<User>()), Times.Once);
        existingUser.Name.Should().Be("OldName");
        existingUser.LastName.Should().Be("OldLastName");
    }
    
    [Fact(DisplayName = "UpdateProfileAsync should return error response when userId is null")]
    public async Task UpdateProfileAsync_Should_Return_Error_When_UserId_Null()
    {
        // Arrange
        const string userId = null!;
        var request = new UpdateUserDtoRequest("NewName", "NewLastName", "new.email@example.com", "+1234567890");

        // Act
        var response = await _updateProfileStrategy.UpdateProfileAsync(request, userId!);

        // Assert
        response.Success.Should().BeFalse();
        response.Message.Should().Be("User not found");
        _userManagerMock.Verify(um => um.FindByIdAsync(userId!), Times.Once);
        _userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact(DisplayName = "UpdateProfileAsync should return error response when profile update fails")]
    public async Task UpdateProfileAsync_Should_Return_Error_When_Update_Fails()
    {
        // Arrange
        const string userId = "12345";
        var request = new UpdateUserDtoRequest("NewName", "NewLastName", "new.email@example.com", "+1234567890");

        var existingUser = new User { Id = userId, Email = "old.email@example.com", PhoneNumber = "+9876543210" };
        existingUser.SetName("OldName");
        existingUser.SetLastName("OldLastName");
        existingUser.SetRole("Admin");
    
        await _appDbContext.Users.AddAsync(existingUser);
        await _appDbContext.SaveChangesAsync();

        _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(existingUser);
        _userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed to update user." }));

        // Act
        var response = await _updateProfileStrategy.UpdateProfileAsync(request, userId);

        // Assert
        response.Success.Should().BeFalse();
        response.Message.Should().Be("Failed to update profile.");
        _userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<User>()), Times.Once);
        Log.Warning($"[PROFILE UPDATE] Failed to update profile for user [{userId}]");
    }
    
    public void Dispose()
    {
        _appDbContext.Database.EnsureDeleted();
        _appDbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}