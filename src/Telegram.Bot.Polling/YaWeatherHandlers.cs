using System.Text.Json;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Infrastructure.Countries;
using Telegram.Bot.Infrastructure.YaWeatherApi;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace Telegram.Bot.Polling;

public static class YaWeatherHandlers
{
    public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var handler = update.Type switch
        {
            UpdateType.Message            => BotOnMessageReceived(botClient, update.Message!),
            _                             => UnknownUpdateHandlerAsync(botClient, update)
        };

        try
        {
            await handler;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(botClient, exception, cancellationToken);
        }
    }

    private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
    {
        Console.WriteLine($"Receive message type: {message.Type}");
        if (message.Type != MessageType.Text)
            return;

        var action = message.Text switch
        {
            "/start"    => SendCityRequest(botClient, message),
            _           => SendWeather(botClient, message)
        };
        Message sentMessage = await action;
        Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");

        static async Task<Message> SendCityRequest(ITelegramBotClient botClient, Message message)
        {
            return await botClient.SendTextMessageAsync(message.Chat.Id, "Enter city name", replyMarkup: new ReplyKeyboardRemove());
        }

        static async Task<Message> SendWeather(ITelegramBotClient botClient, Message message)
        {
            const string filePath = "../../../../Telegram.Bot.Infrastructure/Countries/ru.json";
            await using var openStream = File.OpenRead(filePath);
            var cities = (await JsonSerializer.DeserializeAsync<City[]>(openStream))!;
            var chosenCity = cities.FirstOrDefault(c => c.CityName.ToLower().Contains(message.Text!.ToLower()));

            if (chosenCity is null)
            {
                return await botClient.SendTextMessageAsync(message.Chat.Id, "No such city found");
            }
            
            Response response = await YaWeatherApiClient.ProcessWeather(chosenCity);
            return await botClient.SendTextMessageAsync(message.Chat.Id,
                $"Temperature: {response.Fact.Temp}°\n" +
                $"Feels like: {response.Fact.FeelsLike}°\n" +
                $"Condition: {response.Fact.Condition}\n" +
                $"Wind speed: {response.Fact.WindSpeed} m/s\n" +
                $"Wind gust: {response.Fact.WindGust} m/s\n" +
                $"Wind direction: {response.Fact.WindDir}\n" +
                $"Humidity: {response.Fact.Humidity}%");
        }
    }

    private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
    {
        Console.WriteLine($"Unknown update type: {update.Type}");
        return Task.CompletedTask;
    }
}
