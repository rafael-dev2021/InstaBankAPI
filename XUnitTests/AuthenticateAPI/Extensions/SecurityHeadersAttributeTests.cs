using AuthenticateAPI.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace XUnitTests.AuthenticateAPI.Extensions;

public class SecurityHeadersAttributeTests
{
    [Fact]
    public void OnResultExecuting_ShouldAddSecurityHeaders_WhenNotPresent()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();
        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

        var resultExecutingContext = new ResultExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new EmptyResult(),
            new Mock<Controller>().Object);

        var securityHeadersAttribute = new SecurityHeadersAttribute();

        // Act
        securityHeadersAttribute.OnResultExecuting(resultExecutingContext);

        // Assert
        Assert.True(httpContext.Response.Headers.ContainsKey("X-Content-Type-Options"));
        Assert.Equal("nosniff", httpContext.Response.Headers.XContentTypeOptions);

        Assert.True(httpContext.Response.Headers.ContainsKey("X-Frame-Options"));
        Assert.Equal("SAMEORIGIN", httpContext.Response.Headers.XFrameOptions);

        Assert.True(httpContext.Response.Headers.ContainsKey("X-XSS-Protection"));
        Assert.Equal("1; mode=block", httpContext.Response.Headers.XXSSProtection);

        Assert.True(httpContext.Response.Headers.ContainsKey("Referrer-Policy"));
        Assert.Equal("no-referrer", httpContext.Response.Headers["Referrer-Policy"]);
    }

    [Fact]
    public void OnResultExecuting_ShouldNotOverrideExistingHeaders()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Headers.Append("X-Content-Type-Options", "existing-value");

        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();
        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

        var resultExecutingContext = new ResultExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new EmptyResult(),
            new Mock<Controller>().Object);

        var securityHeadersAttribute = new SecurityHeadersAttribute();

        // Act
        securityHeadersAttribute.OnResultExecuting(resultExecutingContext);

        // Assert
        Assert.True(httpContext.Response.Headers.ContainsKey("X-Content-Type-Options"));
        Assert.Equal("existing-value", httpContext.Response.Headers.XContentTypeOptions);

        Assert.True(httpContext.Response.Headers.ContainsKey("X-Frame-Options"));
        Assert.Equal("SAMEORIGIN", httpContext.Response.Headers.XFrameOptions);

        Assert.True(httpContext.Response.Headers.ContainsKey("X-XSS-Protection"));
        Assert.Equal("1; mode=block", httpContext.Response.Headers.XXSSProtection);

        Assert.True(httpContext.Response.Headers.ContainsKey("Referrer-Policy"));
        Assert.Equal("no-referrer", httpContext.Response.Headers["Referrer-Policy"]);
    }
}