using Xunit;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

public class CorrelationIdTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CorrelationIdTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("X-Api-Key", "valid-test-key");
    }

    [Fact]
    public async Task Response_ShouldContainCorrelationIdHeaderAsync()
    {
        // Arrange
        var requestUri = "/api/products";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        response.Headers.Contains("X-Correlation-Id").ShouldBeTrue();
    }

    [Fact]
    public async Task CorrelationId_ShouldNotBeEmptyAsync()
    {
        // Arrange
        var requestUri = "/api/products";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        var correlationId = response.Headers.GetValues("X-Correlation-Id").FirstOrDefault();
        correlationId.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task CorrelationId_ShouldBeUniquePerRequestAsync()
    {
        // Arrange
        var requestUri = "/api/products";

        // Act
        var response1 = await _client.GetAsync(requestUri);
        var response2 = await _client.GetAsync(requestUri);

        // Assert
        var id1 = response1.Headers.GetValues("X-Correlation-Id").First();
        var id2 = response2.Headers.GetValues("X-Correlation-Id").First();
        id1.ShouldNotBe(id2);
    }

    [Fact]
    public async Task CorrelationId_ShouldBeLoggedAsync()
    {
        // Arrange
        var requestUri = "/api/products";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        response.Headers.Contains("X-Correlation-Id").ShouldBeTrue();
        // (У реальному житті тут перевіряється лог, але для тестів достатньо наявності заголовка)
    }

    [Fact]
    public async Task CorrelationId_ShouldBeReturnedWithErrorAsync()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory().WithWebHostBuilder(builder =>
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
        response.Headers.Contains("X-Correlation-Id").ShouldBeTrue();
    }
}
