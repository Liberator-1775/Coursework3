using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TelegramApi.Services;

public class ConfigureWebhook : IHostedService
{
    private readonly ILogger<ConfigureWebhook> _logger;
    private readonly IServiceProvider _services;
    private readonly BotConfiguration _botConfig;

    public ConfigureWebhook(ILogger<ConfigureWebhook> logger, IServiceProvider services, BotConfiguration botConfig)
    {
        _logger = logger;
        _services = services;
        _botConfig = botConfig;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        var webhookAddress = _botConfig.HostAddress; // secret from azure
        _logger.LogInformation("Setting webhook: {WebhookAddress}", webhookAddress);
        await botClient.SetWebhookAsync(
            url: webhookAddress,
            allowedUpdates: Array.Empty<UpdateType>(),
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        
        _logger.LogInformation("Removing webhook");
        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}