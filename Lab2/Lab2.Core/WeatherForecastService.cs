namespace Lab2.Core;

public class WeatherForecastService
{
    private readonly IWeatherApiClient _api;
    private readonly ICacheService _cache;

    public WeatherForecastService(IWeatherApiClient api, ICacheService cache)
    {
        _api = api;
        _cache = cache;
    }

    public async Task<IEnumerable<WeatherData>> GetForecastAsync(string city, int days)
    {
        var key = $"weather:{city}:{days}";

        if (_cache.Exists(key))
            return _cache.Get<IEnumerable<WeatherData>>(key);

        var forecast = await _api.GetForecastAsync(city, days);
        _cache.Set(key, forecast, TimeSpan.FromMinutes(30));
        return forecast;
    }
}
