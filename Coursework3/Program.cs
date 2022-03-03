using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace Coursework3;

class Program
{
    private static readonly HttpClient client = new HttpClient();
    public static async Task Main(string[] args)
    {
        // 
        /*var botClient = new TelegramBotClient("5149015394:AAH6RVbpwZtScooEBSqatCFEdIayYueXmAw");
        var me = await botClient.GetMeAsync();
        Console.WriteLine($"Hello, world! I am user {me.Id} and my name is {me.FirstName}");*/
        var botClient = new TelegramBotClient("5149015394:AAH6RVbpwZtScooEBSqatCFEdIayYueXmAw");

        using var cts = new CancellationTokenSource();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { } // receive all update types
        };
        var forecast = JsonSerializer.Deserialize<Response>(ProcessWeather().Result);

        botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken: cts.Token);

        var me = await botClient.GetMeAsync(cancellationToken: cts.Token);
        
        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();

// Send cancellation request to stop bot
        cts.Cancel();
        async Task HandleUpdateAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Type != UpdateType.Message)
                return;
            // Only process text messages
            if (update.Message!.Type != MessageType.Text)
                return;

            var chatId = update.Message.Chat.Id;
            
            // Echo received message text
            Message sentMessage = await telegramBotClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Погода в Нижнем Новгороде:\n" + forecast!.fact.temp + "°C",
                cancellationToken: cancellationToken);
        }
        
        Task HandleErrorAsync(ITelegramBotClient telegramBotClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
    }

    private static async Task<string> ProcessWeather()
    {
        client.DefaultRequestHeaders.Add("X-Yandex-API-Key", "fe7fa8d8-016c-4cb5-a9f1-884c9a13ee63");
        var stringTask = await client.GetStringAsync("https://api.weather.yandex.ru/v2/informers?lat=56.326887&lon=44.005986");
        await File.WriteAllTextAsync("../../../weather.json", stringTask);
        return stringTask;
    }
}