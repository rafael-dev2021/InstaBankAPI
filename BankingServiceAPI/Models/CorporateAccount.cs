namespace BankingServiceAPI.Models;

public class CorporateAccount : BaseEntity
{
    public string? Cnpj { get; private set; }
    public string? BusinessName { get; private set; }
    public void SetCnpj(string? cnpj) => Cnpj = cnpj;
    public void SetBusinessName(string? businessName) => BusinessName = businessName;
}