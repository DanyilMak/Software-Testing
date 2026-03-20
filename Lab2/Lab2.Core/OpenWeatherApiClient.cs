using System.Net.Http;
using System.Net.Http.Json;

namespace Lab2.Core;

public class OpenWeatherApiClient : IWeatherApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenWeatherApiClient(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<WeatherData> GetCurrentWeatherAsync(string city)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
        var response = await _httpClient.GetFromJsonAsync<OpenWeatherResponse>(url);

        return new WeatherData(
            response.Name,
            response.Main.Temp,
            response.Weather.First().Description,
            DateTime.UtcNow
        );
    }

    public async Task<IEnumerable<WeatherData>> GetForecastAsync(string city, int days)
    {
        var url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_apiKey}&units=metric";
        var response = await _httpClient.GetFromJsonAsync<OpenWeatherForecastResponse>(url);

        return response.List.Take(days).Select(item =>
            new WeatherData(
                response.City.Name,
                item.Main.Temp,
                item.Weather.First().Description,
                DateTimeOffset.FromUnixTimeSeconds(item.Dt).DateTime
            )
        );
    }
}
