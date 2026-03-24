using Xunit;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
public class ProductsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("X-Api-Key", "valid-test-key");
    }

    [Fact]
    public async Task GetProducts_ReturnsAllSeededProductsAsync()
    {
        // Arrange
        var requestUri = "/api/products";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        products.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetProductById_Existing_Returns200Async()
    {
        // Arrange
        var responseAll = await _client.GetAsync("/api/products");
        var products = await responseAll.Content.ReadFromJsonAsync<List<Product>>();
        var existingId = products!.First().Id;

        // Act
        var response = await _client.GetAsync($"/api/products/{existingId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }


    [Fact]
    public async Task GetProductById_NonExisting_Returns404Async()
    {
        // Arrange
        var requestUri = "/api/products/999";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PostProduct_ValidProduct_Returns201Async()
    {
        // Arrange
        var newProduct = new Product { Name = "Keyboard", Price = 49.99m };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", newProduct);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();
    }

    [Fact]
    public async Task DeleteProduct_Existing_Returns204Async()
    {
        // Arrange
        var requestUri = "/api/products/1";

        // Act
        var response = await _client.DeleteAsync(requestUri);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
