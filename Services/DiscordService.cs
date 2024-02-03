using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MVRB.Configuration;
using Discord.Interactions;

namespace MVRB.Services
{
    public class DiscordService : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly DiscordSocketClient _client;
        private readonly ServiceProvider _services;
        private readonly InteractionService _commands;
        private readonly LoggingService _loggingService;
        public static BotConfig botConfig = new BotConfig();

        public DiscordService()
        {
            _services = ConfigureServices();
            _loggingService = _services.GetRequiredService<LoggingService>();
            _client = _services.GetRequiredService<DiscordSocketClient>();
            _commands = _services.GetRequiredService<InteractionService>();
        }

        public async Task InitAsync()
        {
            await botConfig.InitConfiguration();

            ValidateConfig();

            _client.Log += _loggingService.LogAsync;
            _commands.Log += _loggingService.LogAsync;
            _client.Ready += Ready;

            await RunBotAsync();
        }

        public async Task RunBotAsync()
        {

            await _client.LoginAsync(TokenType.Bot, botConfig.Token);
            await _client.StartAsync();
            await _services.GetRequiredService<CommandService>().InitializeAsync();

            await Task.Delay(-1, _cancellationTokenSource.Token);
        }

        private void ValidateConfig()
        {
            if (string.IsNullOrWhiteSpace(botConfig.Token))
            {
                throw new Exception("Token is missing from the config! Please enter the token.");
            }
        }

        private async Task Ready()
        {
            await _commands.RegisterCommandsGloballyAsync(true);
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<BotConfig>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<CommandService>()
                .AddSingleton<LoggingService>()
                .BuildServiceProvider();
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
    }
}