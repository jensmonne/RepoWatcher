using System.Reflection;
using Discord;

namespace McCoy.Core;

public class BotHostedService : IHostedService
{
    private readonly BotService _botService;

    public BotHostedService(BotService botService)
    {
        _botService = botService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var token = DotNetEnv.Env.GetString("DISCORD_TOKEN");

        _botService.ConfigureEventHandlers();

        await _botService.Client.LoginAsync(TokenType.Bot, token);
        await _botService.Client.StartAsync();

        await _botService.Interaction.AddModulesAsync(Assembly.GetEntryAssembly(), _botService.Services);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _botService.Client.StopAsync();
        await _botService.Client.LogoutAsync();
        _botService.Client.Dispose();
    }
}