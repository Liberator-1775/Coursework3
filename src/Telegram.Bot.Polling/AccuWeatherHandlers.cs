using System.Text.Json;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Infrastructure.AccuWeatherApi;
using Telegram.Bot.Infrastructure.Countries;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace Telegram.Bot.Polling;

public class AccuWeatherHandlers
{
    public static City City { get; set; }

    public static Forecast.ForecastType ForecastType { get; set; }

    public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var handler = update.Type switch
        {
            UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
            UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery!),
            _ => UnknownUpdateHandlerAsync(botClient, update)
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

        var action = message.Text!.Split(' ')[0] switch
        {
            "/start" => Usage(botClient, message),
            "/citySet" => SendReplyKeyboardWithForecastType(botClient, message),
            "Daily" => SendReplyKeyboardWithAvailableDailyIntervals(botClient, message),
            "Hourly" => SendReplyKeyboardWithAvailableHourlyIntervals(botClient, message),
            "OneDay" => SendDailyWeather(botClient, message, Forecast.Daily.OneDay),
            "Five" => SendDailyWeather(botClient, message, Forecast.Daily.Five),
            "OneHour" => SendHourlyWeather(botClient, message, Forecast.Hourly.OneHour),
            "Twelve" => SendHourlyWeather(botClient, message, Forecast.Hourly.Twelve),
            _ => TrySearchCity(botClient, message)
        };
        Message sentMessage = await action;
        Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");

        static async Task<Message> TrySearchCity(ITelegramBotClient botClient, Message message)
        {
            const string filePath = "../../../../Telegram.Bot.Infrastructure/Countries/ru.json";
            await using var openStream = File.OpenRead(filePath);
            var cities = (await JsonSerializer.DeserializeAsync<City[]>(openStream))!;
            var presumptiveCities = cities.Where(c => c.CityName.ToLower().Contains(message.Text!.ToLower())).ToList();
            if (presumptiveCities.Count == 0)
                return await Usage(botClient, message);
            return await SendInlineKeyboard(botClient, message, presumptiveCities);
        }

        static async Task<Message> SendInlineKeyboard(ITelegramBotClient botClient, Message message,
            IEnumerable<City> cities)
        {
            var inlineKeyboardButtons =
                cities.Select(c => new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(c.CityName, c.CityName)
                });
            var inlineKeyboard =
                new InlineKeyboardMarkup(inlineKeyboardButtons);
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                text: "Here's what was found. If there's no your city try again",
                replyMarkup: inlineKeyboard);
        }

        static async Task<Message> SendReplyKeyboardWithForecastType(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new[]
                {
                    new KeyboardButton(Forecast.ForecastType.Daily.ToString()),
                    new KeyboardButton(Forecast.ForecastType.Hourly.ToString()),
                })
            {
                ResizeKeyboard = true
            };

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                text: "Choose forecast type",
                replyMarkup: replyKeyboardMarkup);
        }

        static async Task<Message> SendReplyKeyboardWithAvailableDailyIntervals(ITelegramBotClient botClient,
            Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new[]
                {
                    new KeyboardButton(Forecast.Daily.OneDay.ToString()),
                    new KeyboardButton(Forecast.Daily.Five.ToString()),
                })
            {
                ResizeKeyboard = true
            };

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                text: "Choose daily interval",
                replyMarkup: replyKeyboardMarkup);
        }

        static async Task<Message> SendReplyKeyboardWithAvailableHourlyIntervals(ITelegramBotClient botClient,
            Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new[]
                {
                    new KeyboardButton(Forecast.Hourly.OneHour.ToString()),
                    new KeyboardButton(Forecast.Hourly.Twelve.ToString()),
                })
            {
                ResizeKeyboard = true
            };

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                text: "Choose hourly interval",
                replyMarkup: replyKeyboardMarkup);
        }

        static async Task<Message> SendDailyWeather(ITelegramBotClient botClient, Message message,
            Forecast.Daily dailyForecastInterval)
        {
            var response = await AccuWeatherApiClient.GetDailyForecasts(dailyForecastInterval, City);
            var messageText = string.Empty;
            foreach (var dailyForecast in response.DailyForecasts)
            {
                messageText += $"Date: {dailyForecast.Date.DayOfWeek}\n" +
                               $"Temperature: {dailyForecast.Temperature.Maximum.Value}°C | {dailyForecast.Temperature.Minimum.Value}°C\n" +
                               "Day: ";
                messageText += dailyForecast.Day.HasPrecipitation switch
                {
                    true => $"{dailyForecast.Day.PrecipitationIntensity} {dailyForecast.Day.PrecipitationType}\n",
                    false => "Without precipitation\n"
                };
                messageText += $"{dailyForecast.Day.IconPhrase}\nNight: ";
                messageText += dailyForecast.Night.HasPrecipitation switch
                {
                    true => $"{dailyForecast.Night.PrecipitationIntensity} {dailyForecast.Night.PrecipitationType}\n",
                    false => "Without precipitation\n"
                };
                messageText += $"{dailyForecast.Night.IconPhrase}\n" + "\n";
            }

            return await botClient.SendTextMessageAsync(message.Chat.Id,
                messageText,
                replyMarkup: new ReplyKeyboardRemove());
        }

        static async Task<Message> SendHourlyWeather(ITelegramBotClient botClient, Message message,
            Forecast.Hourly hourlyForecastInterval)
        {
            var response = await AccuWeatherApiClient.GetHourlyForecasts(hourlyForecastInterval, City);
            var messageText = string.Empty;
            foreach (var hourlyResponse in response)
            {
                messageText +=
                    $"{hourlyResponse.DateTime.Hour}:00 : {hourlyResponse.Temperature.Value}°C\nPrecipitation probability: {hourlyResponse.PrecipitationProbability}%\n";
                messageText += hourlyResponse.HasPrecipitation switch
                {
                    true => $"{hourlyResponse.PrecipitationIntensity} {hourlyResponse.PrecipitationType}\n",
                    false => "Without precipitation\n"
                };
                messageText += $"{hourlyResponse.IconPhrase}\n\n";
            }

            return await botClient.SendTextMessageAsync(message.Chat.Id,
                messageText,
                replyMarkup: new ReplyKeyboardRemove());
        }

        static async Task<Message> Usage(ITelegramBotClient botClient, Message message)
        {
            const string usage =
                "Hi! I'm simple weather bot. Send me name of your city and I'll Send current weather in it";

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                text: usage,
                replyMarkup: new ReplyKeyboardRemove());
        }
    }

    private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
        const string filePath = "../../../../Telegram.Bot.Infrastructure/Countries/ru.json";
        await using var openStream = File.OpenRead(filePath);
        var cities = (await JsonSerializer.DeserializeAsync<City[]>(openStream))!;
        City = cities.Single(c => c.CityName.Equals(callbackQuery.Data));
        callbackQuery.Message!.Text = "/citySet";
        await BotOnMessageReceived(botClient, callbackQuery.Message!);
    }

    private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
    {
        Console.WriteLine($"Unknown update type: {update.Type}");
        return Task.CompletedTask;
    }
}