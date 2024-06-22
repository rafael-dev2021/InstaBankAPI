namespace BankingServiceAPI.Models;

public class IndividualAccount : BaseEntity
{
    public string? Cpf { get; private set; }
    public DateTime DateOfBirth { get; private set; }

    public void SetCpf(string? cpf) => Cpf = cpf;
    public void SetDateOfBirth(DateTime dateOfBirth) => DateOfBirth = dateOfBirth;
}