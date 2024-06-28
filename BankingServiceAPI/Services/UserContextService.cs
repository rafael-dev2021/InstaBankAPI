using System.Security.Claims;
using BankingServiceAPI.Models;
using BankingServiceAPI.Services.Interfaces;

namespace BankingServiceAPI.Services;

public class UserContextService : IUserContextService
{
    public async Task<User> GetUserFromHttpContextAsync(HttpContext httpContext)
    {
        await Task.Yield();

        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var name = httpContext.User.FindFirst(ClaimTypes.GivenName)?.Value;
        var lastName = httpContext.User.FindFirst(ClaimTypes.Surname)?.Value;
        var cpf = httpContext.User.FindFirst("Cpf")?.Value;
        var phoneNumber = httpContext.User.FindFirst("PhoneNumber")?.Value;
        var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        var role = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;


        var user = new User();
        user.SetId(userId);
        user.SetName(name);
        user.SetLastName(lastName);
        user.SetCpf(cpf);
        user.SetPhoneNumber(phoneNumber);
        user.SetEmail(email);
        user.SetRole(role);

        return user;
    }
}