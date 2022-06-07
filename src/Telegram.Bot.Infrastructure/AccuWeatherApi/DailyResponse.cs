using System.Text.Json.Serialization;

namespace Telegram.Bot.Infrastructure.AccuWeatherApi;

public class DailyResponse
{
    public DailyForecast[] DailyForecasts { get; set; }
}

public class DailyForecast
{
    public DateTime Date { get; set; }
    public Temperature Temperature { get; set; }
}

public class Temperature
{
    [JsonPropertyName("Minimum")] public MinMax Minimum { get; set; }
    [JsonPropertyName("Maximum")] public MinMax Maximum { get; set; }
}

public class MinMax
{
    public float Value { get; set; }
}

public class DayNight
{
    public bool HasPrecipitation { get; set; }
    public string PrecipitationType { get; set; }
    public string PrecipitationIntensity { get; set; }
}