using BankingServiceAPI.Models;

namespace BankingServiceAPI.Dto.Request;

public record BankAccountDtoRequest(decimal Balance, AccountType AccountType);