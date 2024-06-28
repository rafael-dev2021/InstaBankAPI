using BankingServiceAPI.Exceptions;

namespace BankingServiceAPI.Models;

public sealed class Transfer : BankTransaction
{
    public void Execute()
    {
        if (AccountOrigin!.Balance >= Amount)
        {
            AccountOrigin.SetBalance(AccountOrigin.Balance - Amount);
            AccountDestination!.SetBalance(AccountDestination.Balance + Amount);
        }
        else
        {
            throw new BalanceInsufficientException("Insufficient balance in the origin account.");
        }
    }
}