using BankingServiceAPI.Models;

namespace XUnitTests.BankingServiceAPI.Models;

public class UserTests
{
    [Fact]
    public void SetId_ShouldSetId()
    {
        // Arrange
        var user = new User();
        const string id = "123";

        // Act
        user.SetId(id);

        // Assert
        Assert.Equal(id, user.Id);
    }

    [Fact]
    public void SetName_ShouldSetName()
    {
        // Arrange
        var user = new User();
        const string name = "John";

        // Act
        user.SetName(name);

        // Assert
        Assert.Equal(name, user.Name);
    }

    [Fact]
    public void SetLastName_ShouldSetLastName()
    {
        // Arrange
        var user = new User();
        const string lastName = "Doe";

        // Act
        user.SetLastName(lastName);

        // Assert
        Assert.Equal(lastName, user.LastName);
    }

    [Fact]
    public void SetEmail_ShouldSetEmail()
    {
        // Arrange
        var user = new User();
        const string email = "john.doe@example.com";

        // Act
        user.SetEmail(email);

        // Assert
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public void SetPhoneNumber_ShouldSetPhoneNumber()
    {
        // Arrange
        var user = new User();
        const string phoneNumber = "1234567890";

        // Act
        user.SetPhoneNumber(phoneNumber);

        // Assert
        Assert.Equal(phoneNumber, user.PhoneNumber);
    }

    [Fact]
    public void SetCpf_ShouldSetCpf()
    {
        // Arrange
        var user = new User();
        const string cpf = "12345678901";

        // Act
        user.SetCpf(cpf);

        // Assert
        Assert.Equal(cpf, user.Cpf);
    }

    [Fact]
    public void SetRole_ShouldSetRole()
    {
        // Arrange
        var user = new User();
        const string role = "Admin";

        // Act
        user.SetRole(role);

        // Assert
        Assert.Equal(role, user.Role);
    }
}