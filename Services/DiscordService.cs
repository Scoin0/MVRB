using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MVRB.Configuration;
using Serilog;

namespace MVRB.Services
{
    public class DiscordService
    {
        private readonly DiscordSocketClient _client;
        private readonly ServiceProvider _services;
        private readonly LoggingService _loggingService;
        private BotConfig botConfig;

        public DiscordService()
        {
            _services = ConfigureServices();
            _client = _services.GetRequiredService<DiscordSocketClient>();
            _loggingService = _services.GetRequiredService<LoggingService>();
            botConfig = new BotConfig();
        }

        public async Task InitAsync()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            string discordToken = botConfig.Token;
            if (string.IsNullOrWhiteSpace(discordToken))
            {
                throw new Exception("Token is missing from the config! Please enter the token.");
            }

            _client.Log += _loggingService.LogAsync;

            await _client.LoginAsync(TokenType.Bot, discordToken);
            await _client.StartAsync();

            // Block this task until program is closed.
            await Task.Delay(Timeout.Infinite);
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton< LoggingService>()
                .BuildServiceProvider();
        }
    }
}