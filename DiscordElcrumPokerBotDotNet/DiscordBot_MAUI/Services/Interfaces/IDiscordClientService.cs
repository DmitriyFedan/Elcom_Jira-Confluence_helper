using Discord.WebSocket;

namespace MauiDiscordBot.Services.Interfaces
{
    public interface IDiscordClientService
    {
        DiscordSocketClient DiscordClient { get; }
    }
}