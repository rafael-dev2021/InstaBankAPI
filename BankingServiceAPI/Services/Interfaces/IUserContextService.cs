using BankingServiceAPI.Models;

namespace BankingServiceAPI.Services.Interfaces;

public interface IUserContextService
{
    Task<User> GetUserFromHttpContextAsync(HttpContext httpContext);
}