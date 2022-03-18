using System.Text.Json;
using Core.Users;
using Infrastructure.Countries;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace TelegramService;

public class Telegram : BackgroundService
{
    private readonly ILogger<Telegram> _logger;
    private List<City>? _cities;
    private IUsersRepository _usersRepository;
    private readonly IServiceProvider _serviceProvider;

    public Telegram(ILogger<Telegram> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _usersRepository = serviceProvider.GetRequiredService<IUsersRepository>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var botClient = new TelegramBotClient("5149015394:AAH6RVbpwZtScooEBSqatCFEdIayYueXmAw");
        using var cts = new CancellationTokenSource();
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }
        };
        string fileName = "ru.json";
        using FileStream openStream = File.OpenRead($"../Infrastructure/Countries/{fileName}");
        _cities = await JsonSerializer.DeserializeAsync<List<City>>(openStream, cancellationToken: cts.Token);
        botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cts.Token);
    }
    
    private async Task HandleUpdateAsync(ITelegramBotClient telegramBotClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message)
            return;
        if (update.Message!.Type != MessageType.Text)
            return;

        var chatId = update.Message.Chat.Id;
        var messageText = update.Message.Text;
        var keyboardButtons = _cities!.Select(city => city.CityName).OrderBy(city => city)
            .Select(dummy => new List<KeyboardButton> {dummy}).ToList();

        switch (messageText)
        {
            case "/start":
            {
                Message sentMessage = await telegramBotClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Type first letter of city to be set as default",
                    cancellationToken: cancellationToken);
                break;
            }
            case {Length: 1} c:
            {
                ReplyKeyboardMarkup replyKeyboardMarkup =
                    new ReplyKeyboardMarkup(keyboardButtons.Select(list =>
                        list.Where(button => button.Text[0] == char.Parse(c))));
                Message sentMessage = await telegramBotClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Choose of the cities below",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);
                break;
            }
            case { } city when _cities!.Exists(city1 => city1.CityName == city):
            {
                UserDb user = new UserDb {Id = update.Message.From!.Id, DefaultCity = city};
                await _usersRepository.CreateAsync(user);
                break;
            }
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient telegramBotClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogError(errorMessage);
        return Task.CompletedTask;
    }
}