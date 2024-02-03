using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Serilog;
using System.Reflection;

// I'm dumb. I could not understand commands. This comes from https://www.gngrninja.com/code/2019/4/1/c-discord-bot-command-handling
// TODO: Write my own?

namespace MVRB.Services
{
    public class CommandService
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;

        public CommandService(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.InteractionCreated += HandleInteraction;

            _commands.SlashCommandExecuted += SlashCommandExecuted;
            _commands.ContextCommandExecuted += ContextCommandExecuted;
            _commands.ComponentCommandExecuted += ComponentCommandExecuted;
        }

        private async Task ComponentCommandExecuted(ComponentCommandInfo info, Discord.IInteractionContext context, IResult result)
        {
            if (!result.IsSuccess)
            {
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        await context.Interaction.RespondAsync($"Unmet Precondition: {result.ErrorReason}", null, false, true);
                        break;
                    case InteractionCommandError.UnknownCommand:
                        await context.Interaction.RespondAsync("Unknown Command", null, false, true);
                        break;
                    case InteractionCommandError.BadArgs:
                        await context.Interaction.RespondAsync("Invalid number or arguments", null, false, true);
                        break;
                    case InteractionCommandError.Exception:
                        await context.Interaction.RespondAsync($"Command Exception: {result.ErrorReason}", null, false, true);
                        break;
                    case InteractionCommandError.Unsuccessful:
                        await context.Interaction.RespondAsync("Command could not be executed", null, false, true);
                        break;
                    default:
                        break;
                }
            }
        }

        private async Task ContextCommandExecuted(ContextCommandInfo info, Discord.IInteractionContext context, IResult result)
        {
            if (!result.IsSuccess)
            {
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        await context.Interaction.RespondAsync($"Unmet Precondition: {result.ErrorReason}", null, false, true);
                        break;
                    case InteractionCommandError.UnknownCommand:
                        await context.Interaction.RespondAsync("Unknown Command", null, false, true);
                        break;
                    case InteractionCommandError.BadArgs:
                        await context.Interaction.RespondAsync("Invalid number or arguments", null, false, true);
                        break;
                    case InteractionCommandError.Exception:
                        await context.Interaction.RespondAsync($"Command Exception: {result.ErrorReason}", null, false, true);
                        break;
                    case InteractionCommandError.Unsuccessful:
                        await context.Interaction.RespondAsync("Command could not be executed", null, false, true);
                        break;
                    default:
                        break;
                }
            }
        }

        private async Task SlashCommandExecuted(SlashCommandInfo info, Discord.IInteractionContext context, IResult result)
        {
            if (!result.IsSuccess)
            {
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        await context.Interaction.RespondAsync($"Unmet Precondition: {result.ErrorReason}", null, false, true);
                        break;
                    case InteractionCommandError.UnknownCommand:
                        await context.Interaction.RespondAsync("Unknown Command", null, false, true);
                        break;
                    case InteractionCommandError.BadArgs:
                        await context.Interaction.RespondAsync("Invalid number or arguments", null, false, true);
                        break;
                    case InteractionCommandError.Exception:
                        await context.Interaction.RespondAsync($"Command Exception: {result.ErrorReason}", null, false, true);
                        break;
                    case InteractionCommandError.Unsuccessful:
                        await context.Interaction.RespondAsync("Command could not be executed", null, false, true);
                        break;
                    default:
                        break;
                }
            }
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                var context = new SocketInteractionContext(_client, arg);
                await _commands.ExecuteCommandAsync(context, _services);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);

                if (arg.Type == InteractionType.ApplicationCommand)
                {
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
                }
            }
        } 
    }
}