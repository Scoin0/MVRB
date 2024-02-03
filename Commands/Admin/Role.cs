using Discord;
using Discord.Interactions;
using MVRB.Services;

namespace MVRB.Commands.Admin
{
    [Group("admin", "Admin Commands")]
    public class Role : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService? Commands { get; set; }
        private readonly Services.CommandService _service;

        public Role (Services.CommandService service)
        {
            _service = service;
        }

        
        [SlashCommand("addrole", "Adds the role and request limit")]
        public async Task RoleAsync(IRole role, int requestLimit)
        {
            DiscordService.botConfig.AddRoleTier(role.Id, requestLimit);
            await RespondAsync($"You have added {role.Mention} with a limit of {requestLimit}!");
        }

        [SlashCommand("addchannel", "Adds the channel and request limit")]
        public async Task ChannelAsync(IChannel channel)
        {
            DiscordService.botConfig.AddConfiguredChannelIds(channel.Id);
            await RespondAsync($"You have added {channel}!");
        }
    }
}
