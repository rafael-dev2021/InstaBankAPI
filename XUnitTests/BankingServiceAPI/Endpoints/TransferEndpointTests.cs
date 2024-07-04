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

public class TransferEndpointTests
{
    private readonly Mock<ITransferDtoService> _transferServiceMock = new();
    private readonly Mock<IValidator<TransferDtoRequestByAccount>> _transferByAccountValidatorMock = new();
    private readonly Mock<IValidator<TransferDtoRequestByCpf>> _transferByCpfValidatorMock = new();

    [Fact]
    public async Task TransferByAccountNumber_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var request = new TransferDtoRequestByAccount(123, 456, 100m);
        var response = new TransferByBankAccountNumberDtoResponse(1, "John", "Doe", 456, 100m, DateTime.Now);

        _transferServiceMock.Setup(s =>
                s.TransferByBankAccountNumberDtoAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<decimal>()))
            .ReturnsAsync(response);

        var validationResult = new ValidationResult();
        _transferByAccountValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        app.MapTransferEndpoint();

        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "User") }, "TestAuthType"))
        };

        // Act
        var result = await TransferEndpointTestsHelper.InvokePostEndpoint(
            _transferServiceMock.Object,
            _transferByAccountValidatorMock.Object,
            async (service, userId, req) => await service.TransferByBankAccountNumberDtoAsync(userId,
                req.OriginAccountNumber, req.DestinationAccountNumber, req.Amount),
            request,
            context
        );

        // Assert
        var okResult = Assert.IsType<Ok<object>>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task TransferByCpf_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var request = new TransferDtoRequestByCpf("123.456.789-10", "098.765.432-10", 100m);
        var response = new TransferByCpfDtoResponse(1, "Jane", "Doe", "098.765.432-10", 100m, DateTime.Now);

        _transferServiceMock.Setup(s =>
                s.TransferByCpfDtoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<decimal>()))
            .ReturnsAsync(response);

        var validationResult = new ValidationResult();
        _transferByCpfValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        app.MapTransferEndpoint();

        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "User") }, "TestAuthType"))
        };

        // Act
        var result = await TransferEndpointTestsHelper.InvokePostEndpoint(
            _transferServiceMock.Object,
            _transferByCpfValidatorMock.Object,
            async (service, userId, req) =>
                await service.TransferByCpfDtoAsync(userId, req.OriginCpf!, req.DestinationCpf, req.Amount),
            request,
            context
        );

        // Assert
        var okResult = Assert.IsType<Ok<object>>(result);
        Assert.Equal(response, okResult.Value);
    }

    private static class TransferEndpointTestsHelper
    {
        public static async Task<IResult> InvokePostEndpoint<T>(
            ITransferDtoService service,
            IValidator<T> validator,
            Func<ITransferDtoService, string, T, Task<object>> handler,
            T request,
            HttpContext context) where T : class
        {
            try
            {
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var userId = context.User.FindFirst(ClaimTypes.Name)?.Value;
                if (userId == null)
                {
                    return Results.Unauthorized();
                }

                var response = await handler(service, userId, request);
                return Results.Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        }
    }
}