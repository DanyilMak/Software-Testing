namespace Lab2.Core;

public class OpenWeatherResponse
{
    public string Name { get; set; }
    public MainInfo Main { get; set; }
    public List<WeatherInfo> Weather { get; set; }
}

public class OpenWeatherForecastResponse
{
    public CityInfo City { get; set; }
    public List<ForecastItem> List { get; set; }
}

public class MainInfo
{
    public double Temp { get; set; }
}

public class WeatherInfo
{
    public string Description { get; set; }
}

public class CityInfo
{
    public string Name { get; set; }
}

public class ForecastItem
{
    public long Dt { get; set; }
    public MainInfo Main { get; set; }
    public List<WeatherInfo> Weather { get; set; }
}
