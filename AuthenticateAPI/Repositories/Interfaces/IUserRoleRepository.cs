namespace AuthenticateAPI.Repositories.Interfaces;

public interface IUserRoleRepository
{
    Task UserAsync();
    Task RoleAsync();
}