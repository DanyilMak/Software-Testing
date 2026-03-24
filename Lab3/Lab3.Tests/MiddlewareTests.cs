using Xunit;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Text.Json;

public class MiddlewareTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public MiddlewareTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Request_WithoutApiKey_Returns401Async()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/products");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Request_WithInvalidApiKey_Returns403Async()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "wrong-key");

        // Act
        var response = await client.GetAsync("/api/products");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Request_WithValidApiKey_Returns200Async()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "valid-test-key");

        // Act
        var response = await client.GetAsync("/api/products");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UnhandledException_ReturnsStructuredJsonErrorAsync()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IProductRepository, ThrowingRepository>();
            });
        });
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "valid-test-key");

        // Act
        var response = await client.GetAsync("/api/products");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("error").GetString().ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Pipeline_ShouldProcessInCorrectOrderAsync()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "valid-test-key");

        // Act
        var response = await client.GetAsync("/api/products");

        // Assert
        response.Headers.Contains("X-Correlation-Id").ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
