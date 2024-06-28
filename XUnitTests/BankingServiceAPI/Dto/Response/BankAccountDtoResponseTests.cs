using BankingServiceAPI.Dto.Response;
using FluentAssertions;

namespace XUnitTests.BankingServiceAPI.Dto.Response;

public class BankAccountDtoResponseTests
{
    [Fact]
    public void Should_Return_Correct_Values_From_Properties()
    {
        // Arrange
        const int id = 1;
        const int accountNumber = 123456;
        const decimal balance = 1000.50m;
        const int agency = 789;
        const string accountType = "Savings";
        
        var userDtoResponse = new UserDtoResponse(
            "123ds43d4",
            "John Doe",
            "john.doe@example.com",
            "123.545.899-10",
            "test@localhost.com",
            "+5540028922",
            "User");

        // Act
        var bankAccountDtoResponse =
            new BankAccountDtoResponse(id, accountNumber, balance, agency, accountType, userDtoResponse);

        // Assert
        bankAccountDtoResponse.Id.Should().Be(id);
        bankAccountDtoResponse.AccountNumber.Should().Be(accountNumber);
        bankAccountDtoResponse.Balance.Should().Be(balance);
        bankAccountDtoResponse.Agency.Should().Be(agency);
        bankAccountDtoResponse.AccountType.Should().Be(accountType);
        bankAccountDtoResponse.UserDtoResponse.Should().Be(userDtoResponse);
    }
}