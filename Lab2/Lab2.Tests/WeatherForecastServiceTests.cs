using Lab2.Core;
using NSubstitute;
using Shouldly;
using Xunit;

public class WeatherForecastServiceTests
{
    [Fact]
    public async Task GetForecastAsync_WhenCacheExists_ReturnsCachedData()
    {
        // Arrange
        var api = Substitute.For<IWeatherApiClient>();
        var cache = Substitute.For<ICacheService>();
        var cachedData = new List<WeatherData> { new("Kyiv", 10, "Sunny", DateTime.Now) };
        cache.Exists("weather:Kyiv:3").Returns(true);
        cache.Get<IEnumerable<WeatherData>>("weather:Kyiv:3").Returns(cachedData);
        var sut = new WeatherForecastService(api, cache);

        // Act
        var result = await sut.GetForecastAsync("Kyiv", 3);

        // Assert
        result.ShouldBe(cachedData);
        await api.DidNotReceive().GetForecastAsync(Arg.Any<string>(), Arg.Any<int>());
    }

    [Fact]
    public async Task GetForecastAsync_WhenCacheDoesNotExist_CallsApiAndStoresResult()
    {
        // Arrange
        var api = Substitute.For<IWeatherApiClient>();
        var cache = Substitute.For<ICacheService>();
        var forecast = new List<WeatherData> { new("Kyiv", 12, "Cloudy", DateTime.Now) };
        cache.Exists("weather:Kyiv:2").Returns(false);
        api.GetForecastAsync("Kyiv", 2).Returns(forecast);
        var sut = new WeatherForecastService(api, cache);

        // Act
        var result = await sut.GetForecastAsync("Kyiv", 2);

        // Assert
        result.ShouldBe(forecast);
        cache.Received(1).Set("weather:Kyiv:2", forecast, TimeSpan.FromMinutes(30));
    }

    [Fact]
    public async Task GetForecastAsync_WhenApiThrowsException_ReturnsCachedDataIfAvailable()
    {
        // Arrange
        var api = Substitute.For<IWeatherApiClient>();
        var cache = Substitute.For<ICacheService>();
        var cachedData = new List<WeatherData> { new("Kyiv", 8, "Rainy", DateTime.Now) };
        cache.Exists("weather:Kyiv:1").Returns(true);
        cache.Get<IEnumerable<WeatherData>>("weather:Kyiv:1").Returns(cachedData);

        // важливо: тут ми налаштовуємо мок, щоб він кидав виняток
        api.GetForecastAsync("Kyiv", 1).Returns<Task<IEnumerable<WeatherData>>>(_ => throw new Exception("API error"));

        var sut = new WeatherForecastService(api, cache);

        // Act
        var result = await sut.GetForecastAsync("Kyiv", 1);

        // Assert
        result.ShouldBe(cachedData);
    }

    [Fact]
    public async Task GetForecastAsync_WhenApiThrowsExceptionAndNoCache_ThrowsException()
    {
        // Arrange
        var api = Substitute.For<IWeatherApiClient>();
        var cache = Substitute.For<ICacheService>();
        cache.Exists("weather:Lviv:1").Returns(false);

        api.GetForecastAsync("Lviv", 1).Returns<Task<IEnumerable<WeatherData>>>(_ => throw new Exception("API error"));

        var sut = new WeatherForecastService(api, cache);

        // Act & Assert
        await Should.ThrowAsync<Exception>(() => sut.GetForecastAsync("Lviv", 1));
    }

    [Fact]
    public async Task GetForecastAsync_CallsApiWithCorrectParameters()
    {
        // Arrange
        var api = Substitute.For<IWeatherApiClient>();
        var cache = Substitute.For<ICacheService>();
        cache.Exists("weather:Odesa:5").Returns(false);
        var forecast = new List<WeatherData> { new("Odesa", 20, "Hot", DateTime.Now) };
        api.GetForecastAsync("Odesa", 5).Returns(forecast);
        var sut = new WeatherForecastService(api, cache);

        // Act
        var result = await sut.GetForecastAsync("Odesa", 5);

        // Assert
        await api.Received(1).GetForecastAsync("Odesa", 5);
        result.ShouldBe(forecast);
    }
}
