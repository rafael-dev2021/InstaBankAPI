using AuthenticateAPI.Dto.Response;

namespace XUnitTests.AuthenticateAPI.Dto.Response;

public class ApiTokensDtoResponseTests
{
    [Fact]
    public void ApiTokensDtoResponse_Creation_ShouldSetProperties()
    {
        // Arrange & Act
        var response = new ApiTokensDtoResponse(true);

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public void ApiTokensDtoResponse_Equality_ShouldBeEqual()
    {
        // Arrange
        var response1 = new ApiTokensDtoResponse(true);
        var response2 = new ApiTokensDtoResponse(true);

        // Act & Assert
        Assert.Equal(response1, response2);
        Assert.True(response1 == response2);
    }

    [Fact]
    public void ApiTokensDtoResponse_Inequality_ShouldNotBeEqual()
    {
        // Arrange
        var response1 = new ApiTokensDtoResponse(true);
        var response2 = new ApiTokensDtoResponse(false);

        // Act & Assert
        Assert.NotEqual(response1, response2);
        Assert.True(response1 != response2);
    }
}