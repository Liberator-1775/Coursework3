using Telegram.Bot;
using Telegram.Bot.Examples.WebHook.Services;
using TelegramApi.Services;

namespace TelegramApi;

public class Startup
{
    private IConfiguration Configuration { get; set; }

    private BotConfiguration BotConfig { get; set; }
    
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        BotConfig = Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHostedService<ConfigureWebhook>();
        services.AddHttpClient("tgwebhook")
            .AddTypedClient<ITelegramBotClient>(client => new TelegramBotClient(BotConfig.BotToken, client));
        services.AddScoped<HandleUpdateService>();
        services.AddControllers().AddNewtonsoftJson();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseCors();

        app.UseEndpoints(endpoints =>
        {
            var token = BotConfig.BotToken;
            endpoints.MapControllerRoute(name: "tgwebhook",
                pattern: $"bot/token",
                new { controller = "Webhook", action = "Post" });
            endpoints.MapControllers();
        });
    }
}