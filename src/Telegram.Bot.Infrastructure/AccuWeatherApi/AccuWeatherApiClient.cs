using System.Text.Json;
using Telegram.Bot.Infrastructure.Countries;

namespace Telegram.Bot.Infrastructure.AccuWeatherApi;

public static class AccuWeatherApiClient
{
    private static readonly HttpClient Client = new HttpClient();

    private static async Task<int> GetLocationKeyByGeoPosition(string lat, string lon)
    {
        var stringTask = await Client.GetStringAsync(
            $"http://dataservice.accuweather.com/locations/v1/cities/geoposition/search?apikey=a52e8OChLEFQbd8rYrksISX5ggoSRLXZ&q={lat}%2C{lon}");
        return int.Parse(stringTask.Split(',').FirstOrDefault(s => s.Contains("\"Key\""))?.Split(':')[1].Trim('"') ?? string.Empty);
    }

    public static async Task<DailyResponse> GetDailyForecasts(Forecast.Daily days, City city)
    {
        var stringTask = await Client.GetStringAsync(
            $"http://dataservice.accuweather.com/forecasts/v1/daily/{(int)days}day/{await GetLocationKeyByGeoPosition(city.Lat, city.Lon)}?apikey=a52e8OChLEFQbd8rYrksISX5ggoSRLXZ&metric=true");
        return JsonSerializer.Deserialize<DailyResponse>(stringTask)!;
    }

    public static async Task<HourlyResponse[]> GetHourlyForecasts(Forecast.Hourly hours, City city)
    {
        var stringTask = await Client.GetStringAsync(
            $"http://dataservice.accuweather.com/forecasts/v1/hourly/{(int)hours}hour/{await GetLocationKeyByGeoPosition(city.Lat, city.Lon)}?apikey=a52e8OChLEFQbd8rYrksISX5ggoSRLXZ&metric=true");
        return JsonSerializer.Deserialize<HourlyResponse[]>(stringTask)!;
    }
}