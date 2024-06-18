using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthenticateAPI.Repositories;

public class UserRoleRepository(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
    : IUserRoleRepository
{
    public async Task CreateRoleIfNotExistsAsync(string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            IdentityRole role = new()
            {
                Name = roleName,
                NormalizedName = roleName.ToUpper()
            };
            await roleManager.CreateAsync(role);
        }
    }

    public async Task CreateUserIfNotExistsAsync(string email, string roleName)
    {
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new User
            {
                Email = email,
                UserName = email,
                NormalizedEmail = email.ToUpper(),
                NormalizedUserName = email.ToUpper(),
                PhoneNumber = "1140028922",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            user.SetName("Rafael");
            user.SetLastName("Silva");
            user.SetCpf("123.456.789-01");
            user.SetRole(roleName);

            var result = await userManager.CreateAsync(user, "@Visual24k+");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }
    }

    public async Task UserAsync()
    {
        await CreateUserIfNotExistsAsync("admin@localhost.com","Admin");
        await CreateUserIfNotExistsAsync("user@localhost.com","User");
    }

    public async Task RoleAsync()
    {
        await CreateRoleIfNotExistsAsync("Admin");
        await CreateRoleIfNotExistsAsync("User");
    }
}