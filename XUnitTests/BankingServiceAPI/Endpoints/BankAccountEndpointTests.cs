using System.Security.Claims;
using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Endpoints;
using BankingServiceAPI.Endpoints.Strategies;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace XUnitTests.BankingServiceAPI.Endpoints;

public class BankAccountEndpointTests
{
    private readonly Mock<IBankAccountDtoService> _bankAccountServiceMock = new();
    private readonly Mock<IValidator<BankAccountDtoRequest>> _bankAccountValidatorMock = new();
    private readonly Mock<IDistributedCache> _cacheMock = new();

    [Fact]
    public async Task GetBankAccounts_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var response = new List<BankAccountDtoResponse>
        {
            new(1, 123, 100m, 123, "Savings",
                new UserDtoResponse("123ds43d4", "John Doe", "john.doe@example.com", "123.545.899-10",
                    "test@localhost.com", "+5540028922", "User")),
            new(2, 123, 100m, 123, "Savings",
                new UserDtoResponse("123ds43d4", "John Doe", "john.doe@example.com", "123.545.899-10",
                    "test@localhost.com", "+5540028922", "User"))
        };

        _bankAccountServiceMock.Setup(s => s.GetEntitiesDtoAsync())
            .ReturnsAsync(response);

        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        app.MapBankAccountEndpoints();

        // Act
        var result = await BankAccountEndpointTestsHelper.InvokeGetEndpoint(
            _bankAccountServiceMock.Object,
            _cacheMock.Object,
            async service => await service.GetEntitiesDtoAsync(),
            "cached_bank_accounts_list"
        );

        // Assert
        var okResult = Assert.IsType<Ok<object>>(result);
        Assert.Equal(response, okResult.Value);
    }


    [Fact]
    public async Task GetBankAccountById_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var response = new BankAccountDtoResponse(
            1, 123, 100m, 123, "Savings",
            new UserDtoResponse("123ds43d4", "John Doe", "john.doe@example.com", "123.545.899-10", "test@localhost.com",
                "+5540028922", "User"));

        _bankAccountServiceMock.Setup(s => s.GetEntityDtoByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(response);

        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        app.MapBankAccountEndpoints();

        // Act
        var result = await BankAccountEndpointTestsHelper.InvokeGetByIdEndpoint(
            _bankAccountServiceMock.Object,
            _cacheMock.Object,
            async (service, id) => (await service.GetEntityDtoByIdAsync(id))!,
            "cached_bank_account_by_id",
            1
        );

        // Assert
        var okResult = Assert.IsType<Ok<object>>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task PostBankAccount_ShouldReturnCreated_WhenSuccessful()
    {
        // Arrange
        var request = new BankAccountDtoRequest(123, AccountType.Savings);
        var response = new BankAccountDtoResponse(1, 123, 100m, 123, "Savings",
            new UserDtoResponse("123ds43d4", "John Doe", "john.doe@example.com", "123.545.899-10", "test@localhost.com",
                "+5540028922", "User"));

        _bankAccountServiceMock.Setup(s => s.AddEntityDtoAsync(request, It.IsAny<HttpContext>()))
            .ReturnsAsync(response);

        var validationResult = new ValidationResult();
        _bankAccountValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        app.MapBankAccountEndpoints();

        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "User") }, "TestAuthType"))
        };

        // Act
        var result = await BankAccountEndpointTestsHelper.InvokePostEndpoint(
            _bankAccountServiceMock.Object,
            _bankAccountValidatorMock.Object,
            _cacheMock.Object,
            async (service, req, ctx) => await service.AddEntityDtoAsync(req, ctx),
            request,
            context
        );

        // Assert
        var createdResult = Assert.IsType<Created<BankAccountDtoResponse>>(result);
        Assert.Equal(response, createdResult.Value);
    }

    [Fact]
    public async Task DeleteBankAccount_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _bankAccountServiceMock.Setup(s => s.DeleteEntityDtoAsync(It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        app.MapBankAccountEndpoints();

        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Name, "Admin"), new Claim(ClaimTypes.Role, "Admin") }, "TestAuthType"));

        // Act
        var result = await BankAccountEndpointTestsHelper.InvokeDeleteEndpoint(
            _bankAccountServiceMock.Object,
            _cacheMock.Object,
            async (service, id) => await service.DeleteEntityDtoAsync(id),
            1,
            context
        );

        // Assert
        Assert.IsType<Ok>(result);
    }

    private static class BankAccountEndpointTestsHelper
    {
        public static async Task<IResult> InvokeGetEndpoint(
            IBankAccountDtoService service,
            IDistributedCache cache,
            Func<IBankAccountDtoService, Task<object>> handler,
            string cacheKey)
        {
            try
            {
                return await BankAccountEndpoint.HandleCacheAsync(cache, cacheKey, () => handler(service));
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        }

        public static async Task<IResult> InvokeGetByIdEndpoint(
            IBankAccountDtoService service,
            IDistributedCache cache,
            Func<IBankAccountDtoService, int?, Task<object>> handler,
            string cacheKey,
            int? id)
        {
            try
            {
                return await BankAccountEndpoint.HandleCacheAsync(cache, cacheKey + id, () => handler(service, id));
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        }

        public static async Task<IResult> InvokePostEndpoint(
            IBankAccountDtoService service,
            IValidator<BankAccountDtoRequest> validator,
            IDistributedCache cache,
            Func<IBankAccountDtoService, BankAccountDtoRequest, HttpContext, Task<BankAccountDtoResponse>> handler,
            BankAccountDtoRequest request,
            HttpContext context)
        {
            try
            {
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                await cache.RemoveAsync("cached_bank_accounts_list");

                var response = await handler(service, request, context);
                return Results.Created($"/v1/bank/accounts/{response.Id}", response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        }

        public static async Task<IResult> InvokeDeleteEndpoint(
            IBankAccountDtoService service,
            IDistributedCache cache,
            Func<IBankAccountDtoService, int?, Task> handler,
            int? id,
            HttpContext context)
        {
            try
            {
                var authResult = AuthenticationRules.CheckAdminRole(context);
                if (authResult != null)
                {
                    return authResult;
                }

                await cache.RemoveAsync("cached_bank_accounts_list");
                await cache.RemoveAsync("cached_bank_account_by_id" + id);

                await handler(service, id);
                return Results.Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
            catch (GetIdNotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
        }
    }
}