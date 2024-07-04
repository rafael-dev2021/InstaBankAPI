using System.Security.Claims;
using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Endpoints;
using BankingServiceAPI.Endpoints.Strategies;
using BankingServiceAPI.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace XUnitTests.BankingServiceAPI.Endpoints;

public class WithdrawEndpointTests
{
    private readonly Mock<IWithdrawDtoService> _withdrawServiceMock = new();
    private readonly Mock<IValidator<WithdrawDtoRequest>> _withdrawValidatorMock = new();

    [Fact]
    public async Task PostWithdraw_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var request = new WithdrawDtoRequest(123, 200m);
        var response = new WithdrawDtoResponse(1, "John", "Doe", "123.545.899-10", 123, 200m, DateTime.Now);

        _withdrawServiceMock.Setup(s => s.WithdrawDtoAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()))
            .ReturnsAsync(response);

        var validationResult = new ValidationResult();
        _withdrawValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        app.MapWithdrawEndpoint();

        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "user123") },
                "TestAuthType"))
        };

        // Act
        var result = await WithdrawEndpointTestsHelper.InvokePostEndpoint(
            _withdrawServiceMock.Object,
            _withdrawValidatorMock.Object,
            async (service, req, _) => await service.WithdrawDtoAsync("user123", req.AccountNumber, req.Amount),
            request,
            context
        );

        // Assert
        var okResult = Assert.IsType<Ok<WithdrawDtoResponse>>(result);
        Assert.Equal(response, okResult.Value);
    }

    private static class WithdrawEndpointTestsHelper
    {
        public static async Task<IResult> InvokePostEndpoint(
            IWithdrawDtoService service,
            IValidator<WithdrawDtoRequest> validator,
            Func<IWithdrawDtoService, WithdrawDtoRequest, HttpContext, Task<WithdrawDtoResponse>> handler,
            WithdrawDtoRequest request,
            HttpContext context)
        {
            var (errorResult, _) = await RequestHandler.HandleRequestAsync(context, request, validator);
            if (errorResult != null)
            {
                return errorResult;
            }

            return await RequestHandler.HandleServiceCallAsync(async () =>
            {
                var withdraw = await handler(service, request, context);
                return Results.Ok(withdraw);
            });
        }
    }
}