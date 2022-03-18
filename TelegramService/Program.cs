using Core.Users;
using Infrastructure;
using Infrastructure.Users;
using Microsoft.EntityFrameworkCore;

namespace TelegramService;

public class Program
{
    public static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                IConfiguration configuration = hostContext.Configuration;
                services.AddHostedService<Telegram>();
                services.AddDbContext<AppDbContext>(builder =>
                    builder.UseNpgsql(configuration.GetConnectionString("Coursework")));
            })
            .Build();

        await host.RunAsync();
    }
}