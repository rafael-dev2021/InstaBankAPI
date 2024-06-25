namespace BankingServiceAPI.Models;

public sealed class Deposit : BankTransaction
{
    public void Execute()
    {
        if (AccountOrigin == null)
        {
            throw new InvalidOperationException("Origin account is required for deposit.");
        }

        AccountOrigin.SetBalance(AccountOrigin.Balance + Amount);
    }
}