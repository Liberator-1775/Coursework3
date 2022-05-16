using System.Text.Json;
using Telegram.Bot.Infrastructure.Countries;

namespace Telegram.Bot.Infrastructure.YaWeatherApi;

public static class YaWeatherApiClient
{
    private static readonly HttpClient Client = new HttpClient();

    public static async Task<Response> ProcessWeather(City city)
    {
        Client.DefaultRequestHeaders.Add("X-Yandex-API-Key", "fe7fa8d8-016c-4cb5-a9f1-884c9a13ee63");
        var stringTask =
            await Client.GetStringAsync($"https://api.weather.yandex.ru/v2/informers?lat={city.Lat}&lon={city.Lon}");
        return JsonSerializer.Deserialize<Response>(stringTask)!;
    }
}