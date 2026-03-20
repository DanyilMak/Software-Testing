namespace Lab2.Core;

public interface IWeatherApiClient
{
    Task<WeatherData> GetCurrentWeatherAsync(string city);
    Task<IEnumerable<WeatherData>> GetForecastAsync(string city, int days);
}
