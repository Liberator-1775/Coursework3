namespace Telegram.Bot.Infrastructure.AccuWeatherApi;

public class Forecast
{
    public enum Daily
    {
        One = 1,
        Ten = 10,
        Fifteen = 15,
        Five = 5
    }

    public enum Hourly
    {
        One = 1,
        Twelve = 12,
        OneHundredTwenty = 120,
        TwentyFour = 24,
        SeventyTwo = 72
    }
}