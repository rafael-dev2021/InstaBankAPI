using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Endpoints;
using AuthenticateAPI.Endpoints.Strategies;
using AuthenticateAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace XUnitTests.AuthenticateAPI.Endpoints;

public class AuthenticateEndpointTests
{
    private readonly Mock<IAuthenticateService> _authenticateServiceMock = new();

    public class GetAllUsersTests : AuthenticateEndpointTests
    {
        [Fact]
        public async Task GetAllUsers_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var usersDtoResponse = new List<UserDtoResponse>
            {
                new("123ds43d4", "John", "Doe", "john.doe@example.com", "User"),
                new("567ds89d4", "Jane", "Doe", "jane.doe@example.com", "Admin")
            };

            _authenticateServiceMock.Setup(s => s.GetAllUsersDtoAsync()).ReturnsAsync(usersDtoResponse);

            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();
            app.MapAuthenticateEndpoints();

            // Act
            var result = await AuthenticateEndpointTestsHelper.InvokeGetEndpoint(
                _authenticateServiceMock.Object,
                async service => await service.GetAllUsersDtoAsync()
            );

            // Assert
            var okResult = Assert.IsType<Ok<IEnumerable<UserDtoResponse>>>(result);
            Assert.Equal(usersDtoResponse, okResult.Value);
        }
    }

    private static class AuthenticateEndpointTestsHelper
    {
        public static async Task<IResult> InvokeGetEndpoint<T>(
            IAuthenticateService service,
            Func<IAuthenticateService, Task<T>> handler)
        {
            try
            {
                var response = await handler(service);
                return Results.Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        }
    }
}