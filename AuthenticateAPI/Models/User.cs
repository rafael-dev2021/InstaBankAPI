using Microsoft.AspNetCore.Identity;

namespace AuthenticateAPI.Models;

public class User : IdentityUser
{
    public string Name { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;

    public void SetName(string name) => Name = name;
    public void SetLastName(string lastName) => LastName = lastName;
    public void SetEmail(string email) => Email = email;
    public void SetPhoneNumber(string phoneNumber) => PhoneNumber = phoneNumber;
    public void SetCpf(string cpf) => Cpf = cpf;
}