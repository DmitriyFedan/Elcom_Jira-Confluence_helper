using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

public class CommandHandler
{
    private readonly DiscordSocketClient _discordClient;
    public CommandService _commands;

    // Retrieve client and CommandService instance via ctor
    public CommandHandler(DiscordSocketClient client, CommandService commandService)
    {
        _discordClient = client;
        _commands = commandService;
       
    }

    public async Task InstallCommands()
    {
        _discordClient.MessageReceived += HandleCommandAsync;

        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

    }
    public async Task HandleCommandAsync(SocketMessage messageParam)
    {
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        int argPos = 0;
        if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_discordClient.CurrentUser, ref argPos))) return;

        var context = new SocketCommandContext(_discordClient, message);

        var result = await _commands.ExecuteAsync(context, argPos, null);

        if (!result.IsSuccess)
            await context.Channel.SendMessageAsync(result.ErrorReason);
    }


    

}