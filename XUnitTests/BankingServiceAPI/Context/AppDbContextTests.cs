using BankingServiceAPI.Context;
using BankingServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace XUnitTests.BankingServiceAPI.Context;

public class AppDbContextTests
{
     private static DbContextOptions<AppDbContext> GetInMemoryDbContextOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }
        
        [Fact]
        public void CanCreateDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();

            // Act
            using (var context = new AppDbContext(options))
            {
                context.Database.EnsureCreated();
            }

            // Assert
            using (var context = new AppDbContext(options))
            {
                Assert.True(context.Database.CanConnect());
            }
        }

        [Fact]
        public void ModelBuilderConfiguresEntities()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();

            // Act
            using var context = new AppDbContext(options);
            var model = context.Model;
            var bankAccountEntity = model.FindEntityType(typeof(BankAccount));
            var bankTransactionEntity = model.FindEntityType(typeof(BankTransaction));
            var transactionLogEntity = model.FindEntityType(typeof(TransactionLog));
            var userEntity = model.FindEntityType(typeof(User));
            
            // Assert
            Assert.NotNull(bankAccountEntity);
            Assert.NotNull(bankTransactionEntity);
            Assert.NotNull(transactionLogEntity);
            Assert.NotNull(userEntity);
        }
        
        [Fact]
        public void CanAddAndRetrieveBankAccount()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();

            var bankAccount = new BankAccount();
            bankAccount.SetId(1);
            bankAccount.SetAccountNumber(13254);
            bankAccount.SetBalance(1000.00m);
            bankAccount.SetAgency(111);
            

            // Act
            using (var context = new AppDbContext(options))
            {
                context.BankAccounts.Add(bankAccount);
                context.SaveChanges();
            }

            // Assert
            using (var context = new AppDbContext(options))
            {
                var retrievedBankAccount = context.BankAccounts.Find(bankAccount.Id);
                Assert.NotNull(retrievedBankAccount);
                Assert.Equal(13254, retrievedBankAccount.AccountNumber);
                Assert.Equal(1000.00m, retrievedBankAccount.Balance);
                Assert.Equal(111, retrievedBankAccount.Agency);
            }
        }

        [Fact]
        public void CanRemoveBankAccount()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            
            var bankAccount = new BankAccount();
            bankAccount.SetId(1);
            bankAccount.SetAccountNumber(13254);
            bankAccount.SetBalance(12.9m);
            bankAccount.SetAgency(111);

            // Act
            using (var context = new AppDbContext(options))
            {
                context.BankAccounts.Add(bankAccount);
                context.SaveChanges();

                context.BankAccounts.Remove(bankAccount);
                context.SaveChanges();
            }

            // Assert
            using (var context = new AppDbContext(options))
            {
                var retrievedBankAccount = context.BankAccounts.Find(bankAccount.Id);
                Assert.Null(retrievedBankAccount);
            }
        }
}