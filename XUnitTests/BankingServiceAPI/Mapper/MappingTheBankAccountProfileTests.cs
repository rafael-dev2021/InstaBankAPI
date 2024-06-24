using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Mapper;
using BankingServiceAPI.Models;

namespace XUnitTests.BankingServiceAPI.Mapper;

public class MappingTheBankAccountProfileTests
{
    private readonly IMapper _mapper;

    public MappingTheBankAccountProfileTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingTheBankAccountProfile());
            cfg.AddProfile(new MappingTheUserProfile()); // Add the user profile here
        });
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void Should_Map_BankAccount_To_BankAccountDtoResponse()
    {
        // Arrange
        var user = new User();
        user.SetId("123");
        user.SetName("John");
        user.SetLastName("John");
        user.SetEmail("john.doe@example.com");
        user.SetPhoneNumber("123456789");
        user.SetCpf("123.456.789-00");
        user.SetRole("Admin");

        var bankAccount = new BankAccount();
        bankAccount.SetId(1);
        bankAccount.SetAccountNumber(123456);
        bankAccount.SetAgency(1234);
        bankAccount.SetBalance(100);
        bankAccount.SetAccountType(AccountType.Savings);
        bankAccount.SetUser(user);

        // Act
        var result = _mapper.Map<BankAccountDtoResponse>(bankAccount);

        // Assert
        Assert.Equal(bankAccount.Id, result.Id);
        Assert.Equal(bankAccount.AccountNumber, result.AccountNumber);
        Assert.Equal(bankAccount.Balance, result.Balance);
        Assert.Equal(bankAccount.Agency, result.Agency);
        Assert.Equal(bankAccount.AccountType.ToString(), result.AccountType);

        // Assert
        Assert.Equal(user.Id, result.UserDtoResponse!.Id);
        Assert.Equal(user.Name, result.UserDtoResponse.Name);
        Assert.Equal(user.LastName, result.UserDtoResponse.LastName);
        Assert.Equal(user.Cpf, result.UserDtoResponse.Cpf);
        Assert.Equal(user.Email, result.UserDtoResponse.Email);
        Assert.Equal(user.PhoneNumber, result.UserDtoResponse.PhoneNumber);
        Assert.Equal(user.Role, result.UserDtoResponse.Role);
    }
}