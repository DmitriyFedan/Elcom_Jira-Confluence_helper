using ElcrumPokerBotDiscord;
using MauiDiscordBot.Services;
using MauiDiscordBot.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscordBot_MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<IDiscordClientService, DiscordClientService>();
            builder.Services.AddSingleton<MessageHandlerService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}