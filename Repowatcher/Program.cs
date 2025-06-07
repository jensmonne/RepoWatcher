using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DotNetEnv;
using McCoy.Core;

namespace Repowatcher;

public class Program
{
    public static async Task Main(string[] args)
    {
        Env.Load();

        var builder = WebApplication.CreateBuilder(args);

        var config = new DiscordSocketConfig
        {
            MessageCacheSize = 500,
            GatewayIntents = GatewayIntents.Guilds |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.MessageContent |
                             GatewayIntents.GuildMembers
        };

        builder.Services.AddSingleton(config);
        builder.Services.AddSingleton<DiscordSocketClient>();
        builder.Services.AddSingleton(provider =>
        {
            var client = provider.GetRequiredService<DiscordSocketClient>();
            return new InteractionService(client.Rest);
        });
        builder.Services.AddSingleton<BotService>(provider =>
        {
            var client = provider.GetRequiredService<DiscordSocketClient>();
            var interaction = provider.GetRequiredService<InteractionService>();
            return new BotService(client, interaction, provider);
        });
        builder.Services.AddHostedService<BotHostedService>();

        var app = builder.Build();

        app.MapPost("/github-push", async (GitHubPushPayload payload, BotService botService) =>
        {
            await botService.HandlePushEventAsync(payload);
            return Results.Ok();
        });

        await app.RunAsync();
    }
}