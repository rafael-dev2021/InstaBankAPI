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

public class DepositEndpointTests
{
     private readonly Mock<IDepositDtoService> _depositServiceMock = new();
    private readonly Mock<IValidator<DepositDtoRequest>> _depositValidatorMock = new();

    [Fact]
    public async Task PostDeposit_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var request = new DepositDtoRequest(123, 200m);
        var response = new DepositDtoResponse(1, "John", "Doe", "123.545.899-10", 123, 200m, DateTime.Now);

        _depositServiceMock.Setup(s => s.DepositDtoAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()))
            .ReturnsAsync(response);

        var validationResult = new ValidationResult();
        _depositValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        app.MapDepositEndpoint();

        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "user123") }, "TestAuthType"))
        };

        // Act
        var result = await DepositEndpointTestsHelper.InvokePostEndpoint(
            _depositServiceMock.Object,
            _depositValidatorMock.Object,
            async (service, req, _) => await service.DepositDtoAsync("user123", req.AccountNumber, req.Amount),
            request,
            context
        );

        // Assert
        var okResult = Assert.IsType<Ok<DepositDtoResponse>>(result);
        Assert.Equal(response, okResult.Value);
    }

    private static class DepositEndpointTestsHelper
    {
        public static async Task<IResult> InvokePostEndpoint(
            IDepositDtoService service,
            IValidator<DepositDtoRequest> validator,
            Func<IDepositDtoService, DepositDtoRequest, HttpContext, Task<DepositDtoResponse>> handler,
            DepositDtoRequest request,
            HttpContext context)
        {
            var (errorResult, _) = await RequestHandler.HandleRequestAsync(context, request, validator);
            if (errorResult != null)
            {
                return errorResult;
            }

            return await RequestHandler.HandleServiceCallAsync(async () =>
            {
                var deposit = await handler(service, request, context);
                return Results.Ok(deposit);
            });
        }
    }
}