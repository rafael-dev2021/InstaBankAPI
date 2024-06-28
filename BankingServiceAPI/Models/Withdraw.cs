using BankingServiceAPI.Exceptions;

namespace BankingServiceAPI.Models;

public sealed class Withdraw : BankTransaction
{
    public void Execute()
    {
        if (AccountOrigin!.Balance >= Amount)
        {
            AccountOrigin.SetBalance(AccountOrigin.Balance - Amount);
        }
        else
        {
            throw new BalanceInsufficientException("Insufficient balance in the origin account.");
        }
    }
}