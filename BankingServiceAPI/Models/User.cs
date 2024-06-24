namespace BankingServiceAPI.Models;

public class User
{
    public string? Id { get; private set; }
    public string? Name { get; private set; }
    public string? LastName { get; private set; }
    public string? Cpf { get; private set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Role { get; private set; }

    public void SetId(string? id) => Id = id;
    public void SetName(string? name) => Name = name;
    public void SetLastName(string? lastName) => LastName = lastName;
    public void SetEmail(string? email) => Email = email;
    public void SetPhoneNumber(string? phoneNumber) => PhoneNumber = phoneNumber;
    public void SetCpf(string? cpf) => Cpf = cpf;
    public void SetRole(string? role) => Role = role;
}