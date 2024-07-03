using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthenticateAPI.Repositories;

public class UserRoleRepository(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
    : IUserRoleRepository
{
    private const string Admin = "Admin";
    private const string User = "User";

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

    public async Task CreateUserIfNotExistsAsync(string email, string roleName, string firstName, string lastName,
        string cpf, string phoneNumber)
    {
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new User
            {
                Email = email,
                UserName = email,
                NormalizedEmail = email.ToUpper(),
                NormalizedUserName = email.ToUpper(),
                PhoneNumber = phoneNumber,
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            user.SetName(firstName);
            user.SetLastName(lastName);
            user.SetCpf(cpf);
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
        await CreateUserIfNotExistsAsync("admin@localhost.com", Admin, Admin, Admin, "123.456.789-00",
            "+5540028922");
        await CreateUserIfNotExistsAsync("user@localhost.com", User, User, User, "987.654.321-01",
            "+5540028923");
    }

    public async Task RoleAsync()
    {
        await CreateRoleIfNotExistsAsync(Admin);
        await CreateRoleIfNotExistsAsync(User);
    }
}