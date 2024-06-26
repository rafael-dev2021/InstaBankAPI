using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Endpoints;
using BankingServiceAPI.Endpoints.Strategies;
using BankingServiceAPI.Models;
using BankingServiceAPI.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace XUnitTests.BankingServiceAPI.Endpoints;

public class BankAccountEndpointTests
{
    private readonly Mock<IBankAccountDtoService> _bankAccountServiceMock = new();
    private readonly Mock<IValidator<BankAccountDtoRequest>> _bankAccountValidatorMock = new();

    [Fact]
    public async Task GetAccounts_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var userDtoResponse = new UserDtoResponse(
            "123ds43d4",
            "John Doe",
            "john.doe@example.com",
            "123.545.899-10",
            "test@localhost.com",
            "+5540028922",
            "User");

        var response = new List<BankAccountDtoResponse>
        {
            new(1, 123, 100m, 123, "Savings", userDtoResponse),
            new(2, 123, 100m, 123, "Savings", userDtoResponse)
        };

        _bankAccountServiceMock.Setup(s => s.GetEntitiesDtoAsync())
            .ReturnsAsync(response);

        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        app.MapBankAccountEndpoints();

        // Act
        var result = await BankAccountEndpointTestsHelper.InvokeGetEndpoint(
            _bankAccountServiceMock.Object,
            async service => await service.GetEntitiesDtoAsync()
        );

        // Assert
        var okResult = Assert.IsType<Ok<object>>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task GetAccountById_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var userDtoResponse = new UserDtoResponse(
            "123ds43d4",
            "John Doe",
            "john.doe@example.com",
            "123.545.899-10",
            "test@localhost.com",
            "+5540028922",
            "User");

        var response = new BankAccountDtoResponse(1, 123, 100m, 123, "Savings", userDtoResponse);

        _bankAccountServiceMock.Setup(s => s.GetEntityDtoByIdAsync(1))
            .ReturnsAsync(response);

        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        app.MapBankAccountEndpoints();

        // Act
        var result = await BankAccountEndpointTestsHelper.InvokeGetEndpoint(
            _bankAccountServiceMock.Object,
            async (service, id) => (await service.GetEntityDtoByIdAsync(id))!,
            1
        );

        // Assert
        var okResult = Assert.IsType<Ok<object>>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task CreateAccount_ShouldReturnCreated_WhenSuccessful()
    {
        // Arrange
        var bankAccountRequest = new BankAccountDtoRequest(100m, AccountType.Current);

        var userDtoResponse = new UserDtoResponse(
            "123ds43d4",
            "John",
            "Doe",
            "123.545.899-10",
            "john.doe@example.com",
            "+5540028922",
            "User"
        );

        var bankAccountResponse = new BankAccountDtoResponse(
            1,
            123456,
            1000m,
            123,
            "Current",
            userDtoResponse
        );

        _bankAccountServiceMock.Setup(s => s.AddEntityDtoAsync(bankAccountRequest, It.IsAny<HttpContext>()))
            .ReturnsAsync(bankAccountResponse);

        _bankAccountValidatorMock.Setup(v => v.ValidateAsync(bankAccountRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        app.MapBankAccountEndpoints();

        // Act
        var result = await BankAccountEndpointTestsHelper.InvokePostEndpoint(
            _bankAccountServiceMock.Object,
            bankAccountRequest,
            _bankAccountValidatorMock.Object,
            async (service, request, context) => await service.AddEntityDtoAsync(request, context),
            StatusCodes.Status201Created
        );

        // Assert
        var createdResult = Assert.IsType<StatusCodeHttpResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
    }

    [Fact]
    public async Task DeleteAccount_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _bankAccountServiceMock.Setup(s =>
                s.DeleteEntityDtoAsync(1))
            .Returns(Task.CompletedTask);

        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        app.MapBankAccountEndpoints();

        // Act
        var result = await BankAccountEndpointTestsHelper.InvokeDeleteEndpoint(
            _bankAccountServiceMock.Object,
            async (service, id) => await service.DeleteEntityDtoAsync(id),
            1
        );

        // Assert
        var okResult = Assert.IsType<Ok>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }


    private static class BankAccountEndpointTestsHelper
    {
        public static async Task<IResult> InvokeGetEndpoint(
            IBankAccountDtoService service,
            Func<IBankAccountDtoService, Task<object>> handler)
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

        public static async Task<IResult> InvokeGetEndpoint(
            IBankAccountDtoService service,
            Func<IBankAccountDtoService, int?, Task<object>> handler,
            int id)
        {
            try
            {
                var response = await handler(service, id);
                return Results.Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        }

        public static async Task<IResult> InvokePostEndpoint<T>(
            IBankAccountDtoService service,
            T request,
            IValidator<T> validator,
            Func<IBankAccountDtoService, T, HttpContext, Task> handler,
            int expectedStatusCode = StatusCodes.Status200OK) where T : class
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var context = new DefaultHttpContext();
                await handler(service, request, context);
                return Results.StatusCode(expectedStatusCode);
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        }

        public static async Task<IResult> InvokeDeleteEndpoint(
            IBankAccountDtoService service,
            Func<IBankAccountDtoService, int?, Task> handler,
            int id)
        {
            try
            {
                await handler(service, id);
                return Results.Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        }
    }
}