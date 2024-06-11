using AuthenticateAPI.Repositories;

namespace AuthenticateAPI.Extensions;

public static class UserRolesData
{
    public static async Task AddUserRolesDataAsync(IApplicationBuilder builder)
    {
        var scope = builder.ApplicationServices.CreateScope();
        var result = scope.ServiceProvider.GetService<IUserRoleRepository>();

        if (result != null)
        {
            await result.RoleAsync();
            await result.UserAsync();
        }
    }
}