using AutoMapper;
using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services;
using BankingServiceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace XUnitTests.BankingServiceAPI.Services;

public class BankAccountDtoServiceTests
{
    private readonly Mock<IBankAccountRepository> _repositoryMock;
    private readonly BankAccountDtoService _bankAccountDtoService;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUserContextService> _userContextServiceMock;

    protected BankAccountDtoServiceTests()
    {
        _repositoryMock = new Mock<IBankAccountRepository>();
        _mapperMock = new Mock<IMapper>();
        _userContextServiceMock = new Mock<IUserContextService>();
        _bankAccountDtoService = new BankAccountDtoService(
            _repositoryMock.Object,
            _mapperMock.Object,
            _userContextServiceMock.Object
        );
    }

    public class GetEntitiesDtoAsyncTests : BankAccountDtoServiceTests
    {
        [Fact]
        public async Task GetEntitiesDtoAsync_ShouldReturnMappedBankAccountDtos_WhenEntitiesExist()
        {
            // Arrange
            var user1 = new User();
            user1.SetId("123");
            user1.SetName("John");
            user1.SetLastName("John");
            user1.SetEmail("john.doe@example.com");
            user1.SetPhoneNumber("123456789");
            user1.SetCpf("123.456.789-00");
            user1.SetRole("Admin");

            var user2 = new User();
            user2.SetId("1234");
            user2.SetName("John");
            user2.SetLastName("John");
            user2.SetEmail("john.doe@example.com");
            user2.SetPhoneNumber("123456781");
            user2.SetCpf("123.456.789-01");
            user2.SetRole("User");

            var bankAccount1 = new BankAccount();
            bankAccount1.SetId(1);
            bankAccount1.SetAccountNumber(123456);
            bankAccount1.SetAgency(1234);
            bankAccount1.SetBalance(100);
            bankAccount1.SetAccountType(AccountType.Current);
            bankAccount1.SetUser(user1);

            var bankAccount2 = new BankAccount();
            bankAccount2.SetId(2);
            bankAccount2.SetAccountNumber(654321);
            bankAccount2.SetAgency(1234);
            bankAccount2.SetBalance(100);
            bankAccount2.SetAccountType(AccountType.Savings);
            bankAccount2.SetUser(user2);

            var bankAccounts = new List<BankAccount> { bankAccount1, bankAccount2 };

            var userDto1 = new UserDtoResponse(
                user1.Id,
                user1.Name,
                user1.LastName,
                user1.Cpf,
                user1.Email,
                user1.PhoneNumber,
                user1.Role
            );

            var userDto2 = new UserDtoResponse(
                user2.Id,
                user2.Name,
                user2.LastName,
                user2.Cpf,
                user2.Email,
                user2.PhoneNumber,
                user2.Role
            );

            var bankAccountDtos = new List<BankAccountDtoResponse>
            {
                new(1, 123456, 100m, 1234, "Current", userDto1),
                new(2, 654321, 100m, 1234, "Savings", userDto2)
            };

            _repositoryMock.Setup(r => r.GetEntitiesAsync()).ReturnsAsync(bankAccounts);
            _mapperMock.Setup(m => m.Map<IEnumerable<BankAccountDtoResponse>>(bankAccounts)).Returns(bankAccountDtos);

            // Act
            var result = await _bankAccountDtoService.GetEntitiesDtoAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bankAccountDtos, result);
        }

        [Fact]
        public async Task GetEntitiesDtoAsync_ShouldReturnEmptyList_WhenNoEntitiesExist()
        {
            // Arrange
            var bankAccounts = new List<BankAccount>();

            _repositoryMock.Setup(r => r.GetEntitiesAsync()).ReturnsAsync(bankAccounts);
            _mapperMock.Setup(m => m.Map<IEnumerable<BankAccountDtoResponse>>(bankAccounts))
                .Returns(new List<BankAccountDtoResponse>());

            // Act
            var result = await _bankAccountDtoService.GetEntitiesDtoAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetEntitiesDtoAsync_ShouldThrowBankAccountDtoServiceException_WhenExceptionOccurs()
        {
            // Arrange
            const string exceptionMessage = "Database error";
            _repositoryMock.Setup(r => r.GetEntitiesAsync()).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BankAccountDtoServiceException>(
                () => _bankAccountDtoService.GetEntitiesDtoAsync()
            );

            Assert.Equal("An unexpected error occurred while processing the request.", exception.Message);
            Assert.Equal(exceptionMessage, exception.InnerException?.Message);
        }
    }

    public class GetEntityDtoByIdAsyncTests : BankAccountDtoServiceTests
    {
        [Fact]
        public async Task GetEntityDtoByIdAsync_ShouldReturnMappedBankAccountDto_WhenEntityExists()
        {
            // Arrange
            var user = new User();
            user.SetId("1");
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
            bankAccount.SetAccountType(AccountType.Current);
            bankAccount.SetUser(user);

            var userDto =
                new UserDtoResponse(
                    "1",
                    "John",
                    "John",
                    "123.456.789-00",
                    "john.doe@example.com",
                    "123456789",
                    "Admin");
            var bankAccountDto = new BankAccountDtoResponse(
                1,
                123456,
                100,
                1234,
                "Current",
                userDto);

            _repositoryMock.Setup(r =>
                r.GetEntityByIdAsync(It.IsAny<int>())).ReturnsAsync(bankAccount);

            _mapperMock.Setup(m =>
                m.Map<BankAccountDtoResponse>(bankAccount)).Returns(bankAccountDto);

            // Act
            var result = await _bankAccountDtoService.GetEntityDtoByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bankAccountDto, result);
        }
    }

    public class AddEntityDtoAsyncTests : BankAccountDtoServiceTests
    {
        [Fact]
        public async Task AddEntityDtoAsync_ShouldAddEntity_WhenValidRequest()
        {
            // Arrange
            var bankAccountDtoRequest = new BankAccountDtoRequest(100, AccountType.Current);

            var user = new User();
            user.SetId("1");
            user.SetName("John");
            user.SetLastName("Doe");
            user.SetEmail("john.doe@example.com");
            user.SetPhoneNumber("123456789");
            user.SetCpf("123.456.789-00");
            user.SetRole("User");

            var httpContext = new DefaultHttpContext();

            _userContextServiceMock.Setup(s => s.GetUserFromHttpContextAsync(httpContext)).ReturnsAsync(user);

            var bankAccount = new BankAccount();
            bankAccount.SetId(1);
            bankAccount.SetAccountNumber(123456);
            bankAccount.SetAgency(1234);
            bankAccount.SetBalance(100);
            bankAccount.SetAccountType(AccountType.Current);
            bankAccount.SetUser(user);

            _mapperMock.Setup(m => m.Map<BankAccount>(bankAccountDtoRequest)).Returns(bankAccount);

            // Act
            await _bankAccountDtoService.AddEntityDtoAsync(bankAccountDtoRequest, httpContext);

            // Assert
            _repositoryMock.Verify(r => r.CreateEntityAsync(bankAccount), Times.Once);
        }

        [Fact]
        public async Task AddEntityDtoAsync_ShouldThrowBankAccountDtoServiceException_WhenErrorOccurs()
        {
            // Arrange
            var bankAccountDtoRequest = new BankAccountDtoRequest(100, AccountType.Current);

            var httpContext = new DefaultHttpContext();

            const string exceptionMessage = "Test exception";
            var innerException = new Exception(exceptionMessage);

            _userContextServiceMock.Setup(s => s.GetUserFromHttpContextAsync(httpContext)).Throws(innerException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BankAccountDtoServiceException>(
                () => _bankAccountDtoService.AddEntityDtoAsync(bankAccountDtoRequest, httpContext)
            );

            Assert.Equal("An unexpected error occurred while processing the request.", exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }
    }

    public class DeleteEntityDtoAsyncTests : BankAccountDtoServiceTests
    {
        [Fact]
        public async Task DeleteEntityDtoAsync_ShouldDeleteEntity_WhenEntityExists()
        {
            // Arrange
            const int entityId = 1;

            var user = new User();
            user.SetId("1");
            user.SetName("John");
            user.SetLastName("John");
            user.SetEmail("john.doe@example.com");
            user.SetPhoneNumber("123456789");
            user.SetCpf("123.456.789-00");
            user.SetRole("Admin");

            var bankAccount = new BankAccount();
            bankAccount.SetId(entityId);
            bankAccount.SetAccountNumber(123456);
            bankAccount.SetAgency(1234);
            bankAccount.SetBalance(100);
            bankAccount.SetAccountType(AccountType.Current);
            bankAccount.SetUser(user);

            _repositoryMock.Setup(r => r.GetEntityByIdAsync(entityId)).ReturnsAsync(bankAccount);
            _repositoryMock.Setup(r => r.DeleteEntityAsync(entityId)).ReturnsAsync(true);

            // Act
            await _bankAccountDtoService.DeleteEntityDtoAsync(entityId);

            // Assert
            _repositoryMock.Verify(r => r.DeleteEntityAsync(entityId), Times.Once);
        }

        [Fact]
        public async Task DeleteEntityDtoAsync_ShouldThrowGetIdNotFoundException_WhenEntityDoesNotExist()
        {
            // Arrange
            const int entityId = 1;

            _repositoryMock.Setup(r => r.GetEntityByIdAsync(entityId)).ReturnsAsync((BankAccount)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<GetIdNotFoundException>(
                () => _bankAccountDtoService.DeleteEntityDtoAsync(entityId)
            );

            Assert.Equal($"Could not find bank account id: '{entityId}' ", exception.Message);
        }
    }
}