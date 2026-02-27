using System.Text.Json;
using LinkwellProductionSystem.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;

namespace LinkwellProductionSystem.Tests;

public class GlobalExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenNoException_CallsNextDelegate()
    {
        // Arrange
        var wasCalled = false;
        RequestDelegate next = context =>
        {
            wasCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new GlobalExceptionMiddleware(
            next,
            NullLogger<GlobalExceptionMiddleware>.Instance,
            new FakeHostEnvironment("Development"));

        var httpContext = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.True(wasCalled);
    }

    [Fact]
    public async Task InvokeAsync_WhenArgumentException_ReturnsBadRequestProblemDetails()
    {
        // Arrange
        RequestDelegate next = _ => throw new ArgumentException("station code is required");

        var middleware = new GlobalExceptionMiddleware(
            next,
            NullLogger<GlobalExceptionMiddleware>.Instance,
            new FakeHostEnvironment("Development"));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/api/station/save";
        httpContext.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
        Assert.Equal("application/problem+json", httpContext.Response.ContentType);

        var problem = await ReadProblemDetailsAsync(httpContext.Response.Body);
        Assert.Equal("Invalid request", problem.Title);
        Assert.Equal("station code is required", problem.Detail);
        Assert.Equal("/api/station/save", problem.Instance);
    }

    [Fact]
    public async Task InvokeAsync_WhenUnauthorizedAccessException_ReturnsUnauthorizedProblemDetails()
    {
        // Arrange
        RequestDelegate next = _ => throw new UnauthorizedAccessException();

        var middleware = new GlobalExceptionMiddleware(
            next,
            NullLogger<GlobalExceptionMiddleware>.Instance,
            new FakeHostEnvironment("Development"));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/admin";
        httpContext.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, httpContext.Response.StatusCode);

        var problem = await ReadProblemDetailsAsync(httpContext.Response.Body);
        Assert.Equal("Unauthorized", problem.Title);
        Assert.Equal("You are not authorized to perform this action.", problem.Detail);
        Assert.Equal("/admin", problem.Instance);
    }

    [Fact]
    public async Task InvokeAsync_WhenGenericExceptionInProduction_HidesExceptionMessage()
    {
        // Arrange
        RequestDelegate next = _ => throw new Exception("sensitive details");

        var middleware = new GlobalExceptionMiddleware(
            next,
            NullLogger<GlobalExceptionMiddleware>.Instance,
            new FakeHostEnvironment("Production"));

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);

        var problem = await ReadProblemDetailsAsync(httpContext.Response.Body);
        Assert.Equal("Internal Server Error", problem.Title);
        Assert.Equal("An unexpected error occurred.", problem.Detail);
    }

    private static async Task<ProblemDetails> ReadProblemDetailsAsync(Stream responseBody)
    {
        responseBody.Position = 0;

        var problem = await JsonSerializer.DeserializeAsync<ProblemDetails>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(problem);
        return problem!;
    }

    private sealed class FakeHostEnvironment(string environmentName) : Microsoft.Extensions.Hosting.IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "LinkwellProductionSystem.Tests";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
