using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Mapper;
using BankingServiceAPI.Models;

namespace XUnitTests.BankingServiceAPI.Mapper;

public class MappingTheProfileDepositTests
{
    private readonly IMapper _mapper;

    public MappingTheProfileDepositTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingTheProfileDeposit());
        });
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void Should_Map_Deposit_To_DepositDtoResponse()
    {
        // Arrange
        var user = new User();
        user.SetId("123");
        user.SetName("John");
        user.SetLastName("Doe");
        user.SetEmail("john.doe@example.com");
        user.SetPhoneNumber("123456789");
        user.SetCpf("123.456.789-00");
        user.SetRole("Admin");

        var accountDestination = new BankAccount();
        accountDestination.SetId(1);
        accountDestination.SetAccountNumber(123456);
        accountDestination.SetAgency(1234);
        accountDestination.SetBalance(100);
        accountDestination.SetAccountType(AccountType.Savings);
        accountDestination.SetUser(user);

        var deposit = new Deposit();
        deposit.SetId(1);
        deposit.SetAccountDestination(accountDestination);
        deposit.SetAmount(500);
        deposit.SetTransferDate(new DateTime(2024, 6, 26));

        // Act
        var result = _mapper.Map<DepositDtoResponse>(deposit);

        // Assert
        Assert.Equal(deposit.Id, result.TransactionId);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.LastName, result.LastName);
        Assert.Equal(user.Cpf, result.Cpf);
        Assert.Equal(accountDestination.AccountNumber, result.BankAccountNumber);
        Assert.Equal(deposit.Amount, result.Amount);
        Assert.Equal(deposit.TransferDate, result.TransferDate);
    }
}