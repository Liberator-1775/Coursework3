namespace Telegram.Bot.Infrastructure.AccuWeatherApi;

public class HourlyResponse
{
    public DateTime DateTime { get; set; }

    public string IconPhrase { get; set; }

    public bool HasPrecipitation { get; set; }

    public HourlyTemperature Temperature { get; set; }

    public string PrecipitationType { get; set; }

    public string PrecipitationIntensity { get; set; }

    public int? PrecipitationProbability { get; set; }
}

public class HourlyTemperature
{
    public float Value { get; set; }
}