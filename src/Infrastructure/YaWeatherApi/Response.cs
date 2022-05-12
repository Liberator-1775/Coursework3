using System.Text.Json.Serialization;

namespace Infrastructure.YaWeatherApi;

public class Response
{
    [JsonPropertyName("now")] public int UnixTimeNow { get; set; }
    [JsonPropertyName("now_dt")] public DateTime Now { get; set; }
    [JsonPropertyName("info")] public Info Info { get; set; }
    [JsonPropertyName("fact")] public Fact Fact { get; set; }
    [JsonPropertyName("forecast")] public Forecast Forecast { get; set; }
}

public class Info
{
    [JsonPropertyName("lat")] public double Lat { get; set; }
    [JsonPropertyName("lon")] public double Lon { get; set; }
    [JsonPropertyName("url")] public string Url { get; set; }
}

public class Fact
{
    [JsonPropertyName("temp")] public int Temp { get; set; }
    [JsonPropertyName("feels_like")] public int FeelsLike { get; set; }
    [JsonPropertyName("icon")] public string Icon { get; set; }
    [JsonPropertyName("condition")] public string Condition { get; set; }
    [JsonPropertyName("wind_speed")] public double WindSpeed { get; set; }
    [JsonPropertyName("wind_gust")] public double WindGust { get; set; }
    [JsonPropertyName("wind_dir")] public string WindDir { get; set; }
    [JsonPropertyName("pressure_mm")] public int PressureMillimeters { get; set; }
    [JsonPropertyName("pressure_pa")] public int PressureHectopascal { get; set; }
    [JsonPropertyName("humidity")] public int Humidity { get; set; }
    [JsonPropertyName("daytime")] public string Daytime { get; set; }
    [JsonPropertyName("polar")] public bool Polar { get; set; }
    [JsonPropertyName("season")] public string Season { get; set; }
    [JsonPropertyName("obs_time")] public int UnixTimeOfMeasurementOfWeatherData { get; set; }
}

public class Forecast
{
    [JsonPropertyName("date")] public DateTime Date { get; set; }
    [JsonPropertyName("date_ts")] public int UnixDate { get; set; }
    [JsonPropertyName("week")] public int Week { get; set; }
    [JsonPropertyName("sunrise")] public string Sunrise { get; set; }
    [JsonPropertyName("sunset")] public string Sunset { get; set; }
    // 0 — полнолуние, 1-3 — убывающая Луна, 4 — последняя четверть, 5-7 — убывающая Луна, 8 — новолуние, 9-11 — растущая Луна, 12 — первая четверть, 13-15 — растущая Луна.
    [JsonPropertyName("moon_code")] public int MoonCode { get; set; }
    [JsonPropertyName("moon_text")] public string MoonText { get; set; }
    [JsonPropertyName("parts")] public Part[] Parts { get; set; }
}

public class Part
{
    [JsonPropertyName("part_name")] public string PartName { get; set; }
    [JsonPropertyName("temp_min")] public int TempMin { get; set; }
    [JsonPropertyName("temp_max")] public int TempMax { get; set; }
    [JsonPropertyName("temp_avg")] public int TempAvg { get; set; }
    [JsonPropertyName("feels_like")] public int FeelsLike { get; set; }
    [JsonPropertyName("icon")] public string Icon { get; set; }
    [JsonPropertyName("condition")] public string Condition { get; set; }
    [JsonPropertyName("daytime")] public string Daytime { get; set; }
    [JsonPropertyName("polar")] public bool Polar { get; set; }
    [JsonPropertyName("wind_speed")] public double WindSpeed { get; set; }
    [JsonPropertyName("wind_gust")] public double WindGust { get; set; }
    [JsonPropertyName("wind_dir")] public string WindDir { get; set; }
    [JsonPropertyName("pressure_mm")] public int PressureMillimeters { get; set; }
    [JsonPropertyName("pressure_pa")] public int PressureHectopascal { get; set; }
    [JsonPropertyName("humidity")] public int Humidity { get; set; }
    [JsonPropertyName("prec_mm")] public int PredictedPrecipitationMillimeters { get; set; }
    [JsonPropertyName("prec_period")] public int PredictedPrecipitationPeriodMinutes { get; set; }
    [JsonPropertyName("prec_prob")] public int ProbabilityOfPrecipitation { get; set; }
}