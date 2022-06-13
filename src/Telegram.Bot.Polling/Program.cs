using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Polling;

public static class Program
{
    private static TelegramBotClient? _bot;

    public static async Task Main()
    {
        _bot = new TelegramBotClient(Configuration.BotToken);

        User me = await _bot.GetMeAsync();

        using var cts = new CancellationTokenSource();

        _bot.StartReceiving(updateHandler: AccuWeatherHandlers.HandleUpdateAsync,
                           errorHandler: AccuWeatherHandlers.HandleErrorAsync,
                           receiverOptions: new ReceiverOptions()
                           {
                               AllowedUpdates = Array.Empty<UpdateType>()
                           },
                           cancellationToken: cts.Token);

        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();
        
        cts.Cancel();
    }
}
