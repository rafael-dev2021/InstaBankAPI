using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Mapper;
using BankingServiceAPI.Models;

namespace XUnitTests.BankingServiceAPI.Mapper;

public class MappingTheTransferProfileTests
{
     private readonly IMapper _mapper;

        public MappingTheTransferProfileTests()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingTheTransferProfile());
            });
            _mapper = configuration.CreateMapper();
        }

        [Fact]
        public void Should_Map_Transfer_To_TransferDtoResponse()
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

            var transfer = new Transfer();
            transfer.SetId(1);
            transfer.SetAccountDestination(accountDestination);
            transfer.SetAmount(500);
            transfer.SetTransferDate(new DateTime(2024, 6, 26));

            // Act
            var result = _mapper.Map<TransferByCpfDtoResponse>(transfer);

            // Assert
            Assert.Equal(transfer.Id, result.TransactionId);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.LastName, result.LastName);
            Assert.Equal(user.Cpf, result.DestinationCpf);
            Assert.Equal(transfer.Amount, result.Amount);
            Assert.Equal(transfer.TransferDate, result.TransferDate);
        }

        [Fact]
        public void Should_Map_Transfer_To_TransferByBankAccountNumberDtoResponse()
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

            var transfer = new Transfer();
            transfer.SetId(1);
            transfer.SetAccountDestination(accountDestination);
            transfer.SetAmount(500);
            transfer.SetTransferDate(new DateTime(2024, 6, 26));

            // Act
            var result = _mapper.Map<TransferByBankAccountNumberDtoResponse>(transfer);

            // Assert
            Assert.Equal(transfer.Id, result.TransactionId);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.LastName, result.LastName);
            Assert.Equal(accountDestination.AccountNumber, result.DestinationBankAccountNumber);
            Assert.Equal(transfer.Amount, result.Amount);
            Assert.Equal(transfer.TransferDate, result.TransferDate);
        }
}