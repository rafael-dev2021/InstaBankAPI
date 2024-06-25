using BankingServiceAPI.Endpoints;

namespace BankingServiceAPI.Extensions;

public static class MapEndpointsDependencyInjection
{
    public static void AddMapEndpointsDependencyInjection(this WebApplication app)
    {
        app.MapBankAccountEndpoints();
        app.MapTransferEndpoint();
        app.MapDepositEndpoint();
        app.MapWithdrawEndpoint();
    }
}