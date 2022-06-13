namespace Telegram.Bot.Infrastructure.AccuWeatherApi;

public static class Forecast
{
    public enum Daily
    {
        OneDay = 1,
        Five = 5
    }

    public enum Hourly
    {
        OneHour = 1,
        Twelve = 12
    }

    public enum ForecastType
    {
        Daily,
        Hourly
    }
}