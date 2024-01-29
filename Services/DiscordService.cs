using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MVRB.Configuration;

namespace MVRB.Services
{
    public class DiscordService : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly DiscordSocketClient _client;
        private readonly ServiceProvider _services;
        private readonly LoggingService _loggingService;
        public BotConfig botConfig = new BotConfig();

        public DiscordService()
        {
            _services = ConfigureServices();
            _loggingService = _services.GetRequiredService<LoggingService>();
            _client = _services.GetRequiredService<DiscordSocketClient>();
        }

        public async Task InitAsync()
        {
            await botConfig.InitConfiguration();

            ValidateConfig();

            _client.Log += _loggingService.LogAsync;

            await RunBotAsync();
        }

        public async Task RunBotAsync()
        {
            await _client.LoginAsync(TokenType.Bot, botConfig.Token);
            await _client.StartAsync();

            await Task.Delay(-1, _cancellationTokenSource.Token);
        }

        public void StopBot()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            _services.Dispose();
            _cancellationTokenSource.Dispose();
        }

        private void ValidateConfig()
        {
            if (string.IsNullOrWhiteSpace(botConfig.Token))
            {
                throw new Exception("Token is missing from the config! Please enter the token.");
            }
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<LoggingService>()
                .AddSingleton<BotConfig>()
                .BuildServiceProvider();
        }
    }
}