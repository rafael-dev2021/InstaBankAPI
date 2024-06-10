namespace AuthenticateAPI.Repositories;

public interface IUserRoleRepository
{
    Task UserAsync();
    Task RoleAsync();
}