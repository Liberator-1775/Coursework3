using System.Text.Json;
using Telegram.Bot.Infrastructure.Countries;

namespace Telegram.Bot.Infrastructure.AccuWeatherApi;

public class AccuWeatherApiClient
{
    private static readonly HttpClient Client = new HttpClient();

    private static async Task<int> GetLocationKeyByGeoPosition(double lat, double lon)
    {
        var stringTask = await Client.GetStringAsync(
            $"http://dataservice.accuweather.com/locations/v1/cities/geoposition/search?apikey=a52e8OChLEFQbd8rYrksISX5ggoSRLXZ&q={lat}%2C{lon}");
        return int.Parse(stringTask.Split(',').FirstOrDefault(s => s.Contains("\"Key\""))?.Split(':')[1].Trim('"') ?? string.Empty);
    }

    public async Task<DailyResponse> GetDailyForecasts(Forecast.Daily days, City city)
    {
        var stringTask = await Client.GetStringAsync(
            $"http://dataservice.accuweather.com/forecasts/v1/daily/{days}day/{GetLocationKeyByGeoPosition(double.Parse(city.Lat), double.Parse(city.Lon))}?apikey=a52e8OChLEFQbd8rYrksISX5ggoSRLXZ&metric=true");
        return JsonSerializer.Deserialize<DailyResponse>(stringTask)!;
    }

    public async Task<HourlyResponse> GetHourlyForecasts(Forecast.Hourly hours, City city)
    {
        var stringTask = await Client.GetStringAsync(
            $"http://dataservice.accuweather.com/forecasts/v1/hourly/{hours}hour/{GetLocationKeyByGeoPosition(double.Parse(city.Lat), double.Parse(city.Lon))}?apikey=a52e8OChLEFQbd8rYrksISX5ggoSRLXZ&metric=true");
        return JsonSerializer.Deserialize<HourlyResponse>(stringTask)!;
    }
}