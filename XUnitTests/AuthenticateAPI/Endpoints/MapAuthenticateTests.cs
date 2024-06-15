using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Endpoints;
using AuthenticateAPI.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace XUnitTests.AuthenticateAPI.Endpoints;

public class MapAuthenticateTests
{
    private readonly Mock<IAuthenticateService> _authenticateServiceMock = new();
    private readonly Mock<IValidator<LoginDtoRequest>> _loginValidatorMock = new();
    private readonly Mock<IValidator<RegisterDtoRequest>> _registerValidatorMock = new();
    private readonly Mock<IValidator<ChangePasswordDtoRequest>> _changePasswordValidatorMock = new();

    [Fact]
    public async Task LoginEndpoint_ShouldReturnOk_WhenValidationPasses()
    {
        // Arrange
        var request = new LoginDtoRequest("test@localhost.com", "@Visual23k+", true);
        var validationResult = new ValidationResult();
        var response = new TokenDtoResponse("token", "refreshToken");

        _loginValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);
        _authenticateServiceMock.Setup(s => s.LoginAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await MapAuthenticateTestsHelper.InvokePostEndpoint(
            _authenticateServiceMock.Object,
            request,
            _loginValidatorMock.Object,
            async (service, req) => await service.LoginAsync(req),
            StatusCodes.Status200OK
        );

        // Assert
        var okResult = Assert.IsType<Ok<object>>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task LoginEndpoint_ShouldReturnBadRequest_WhenAuthenticationFails()
    {
        // Arrange
        var request = new LoginDtoRequest("test@localhost.com", "@Visual23k+", true);
        var validationResult = new ValidationResult();
        const string exceptionMessage = "Invalid email or password. Please try again.";

        _loginValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);
        _authenticateServiceMock.Setup(s => s.LoginAsync(request))
            .ThrowsAsync(new UnauthorizedAccessException(exceptionMessage));

        // Act
        var result = await MapAuthenticateTestsHelper.InvokePostEndpoint(
            _authenticateServiceMock.Object,
            request,
            _loginValidatorMock.Object,
            async (service, req) => await service.LoginAsync(req),
            StatusCodes.Status400BadRequest
        );

        // Assert
        var jsonResult = Assert.IsType<JsonHttpResult<Dictionary<string, string>>>(result);
        var responseData = jsonResult.Value;

        Assert.Equal(exceptionMessage, responseData?["Message"]);
        Assert.Equal(StatusCodes.Status400BadRequest, jsonResult.StatusCode);
    }

    [Fact]
    public async Task RegisterEndpoint_ShouldReturnCreated_WhenValidationPasses()
    {
        // Arrange
        var request = new RegisterDtoRequest("John", "Doe", "1234567890", "123.456.789-10", "test@localhost.com",
            "@Visual23k+", "@Visual23k+");
        var validationResult = new ValidationResult();
        var response = new TokenDtoResponse("token", "refreshToken");

        _registerValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);
        _authenticateServiceMock.Setup(s => s.RegisterAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await MapAuthenticateTestsHelper.InvokePostEndpoint(
            _authenticateServiceMock.Object,
            request,
            _registerValidatorMock.Object,
            async (service, req) => await service.RegisterAsync(req),
            StatusCodes.Status201Created
        );

        // Assert
        var createdResult = Assert.IsType<Created<object>>(result);
        var responseData = createdResult.Value as TokenDtoResponse;

        Assert.NotNull(responseData);
        Assert.Equal(response.Token, responseData.Token);
        Assert.Equal(response.RefreshToken, responseData.RefreshToken);
    }

    [Fact]
    public async Task RegisterEndpoint_ShouldReturnBadRequest_WhenRegistrationFails()
    {
        // Arrange
        var request = new RegisterDtoRequest("John", "Doe", "1234567890", "123.456.789-10", "test@localhost.com",
            "@Visual23k+", "@Visual23k+");
        var validationResult = new ValidationResult();
        const string exceptionMessage = "Registration failed due to some error.";

        _registerValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);
        _authenticateServiceMock.Setup(s => s.RegisterAsync(request))
            .ThrowsAsync(new UnauthorizedAccessException(exceptionMessage));

        // Act
        var result = await MapAuthenticateTestsHelper.InvokePostEndpoint(
            _authenticateServiceMock.Object,
            request,
            _registerValidatorMock.Object,
            async (service, req) => await service.RegisterAsync(req),
            StatusCodes.Status400BadRequest
        );

        // Assert
        var jsonResult = Assert.IsType<JsonHttpResult<Dictionary<string, string>>>(result);
        var responseData = jsonResult.Value;

        Assert.Equal(exceptionMessage, responseData?["Message"]);
        Assert.Equal(StatusCodes.Status400BadRequest, jsonResult.StatusCode);
    }

    [Fact]
    public async Task ChangePasswordEndpoint_ShouldReturnOk_WhenValidationPasses()
    {
        // Arrange
        var request = new ChangePasswordDtoRequest("test@localhost.com", "OldPassword@123", "NewPassword@123");
        var validationResult = new ValidationResult();
        const bool response = true;

        _changePasswordValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);
        _authenticateServiceMock.Setup(s => s.ChangePasswordAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await MapAuthenticateTestsHelper.InvokeAuthorizedPutEndpoint(
            _authenticateServiceMock.Object,
            request,
            _changePasswordValidatorMock.Object,
            async (service, req, _) => await service.ChangePasswordAsync(req),
            null!
        );

        // Assert
        var okResult = Assert.IsType<Ok<object>>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task ChangePasswordEndpoint_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new ChangePasswordDtoRequest("test@localhost.com", "OldPassword@123", "NewPassword@123");
        var validationResult = new ValidationResult();
        const string exceptionMessage = "Registration failed due to some error.";

        _changePasswordValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);
        _authenticateServiceMock.Setup(s => s.ChangePasswordAsync(request))
            .ThrowsAsync(new UnauthorizedAccessException(exceptionMessage));

        // Act
        var result = await MapAuthenticateTestsHelper.InvokeAuthorizedPutEndpoint(
            _authenticateServiceMock.Object,
            request,
            _changePasswordValidatorMock.Object,
            async (service, req, _) => await service.ChangePasswordAsync(req),
            null!
        );

        // Assert
        var jsonResult = Assert.IsType<JsonHttpResult<Dictionary<string, string>>>(result);
        var responseData = jsonResult.Value;

        Assert.Equal(exceptionMessage, responseData?["Message"]);
        Assert.Equal(StatusCodes.Status400BadRequest, jsonResult.StatusCode);
    }

    private static class MapAuthenticateTestsHelper
    {
        public static async Task<IResult> InvokePostEndpoint<T>(
            IAuthenticateService service,
            T request,
            IValidator<T> validator,
            Func<IAuthenticateService, T, Task<object>> handler,
            int expectedStatusCode) where T : class
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                return await MapAuthenticate.HandleRequestAsync(service, handler, request, expectedStatusCode);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Json(new { ex.Message }, statusCode: StatusCodes.Status400BadRequest);
            }
        }

        public static async Task<IResult> InvokeAuthorizedPutEndpoint<T>(
            IAuthenticateService service,
            T request,
            IValidator<T> validator,
            Func<IAuthenticateService, T, string, Task<object>> handler,
            string userId) where T : class
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                return await MapAuthenticate.HandleAuthorizedRequestAsync(service, handler, request, userId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Json(new { ex.Message }, statusCode: StatusCodes.Status400BadRequest);
            }
        }
    }
}