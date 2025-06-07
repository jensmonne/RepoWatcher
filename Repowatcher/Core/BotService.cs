using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Repowatcher;

namespace McCoy.Core;

public class BotService
{
    public DiscordSocketClient Client { get; }
    public InteractionService Interaction { get; }
    public IServiceProvider Services { get; }

    public BotService(DiscordSocketClient client, InteractionService interaction, IServiceProvider services)
    {
        Client = client;
        Interaction = interaction;
        Services = services;
    }

    public void ConfigureEventHandlers()
    {
        Client.Ready += () =>
        {
            Console.WriteLine($"Logged in as {Client.CurrentUser.Username}#{Client.CurrentUser.Discriminator}");
            return Task.CompletedTask;
        };
    }

    public async Task HandlePushEventAsync(GitHubPushPayload payload)
    {
        var channel = Client.GetChannel(1380559484501360781) as IMessageChannel;
        if (channel == null) return;

        var embed = new EmbedBuilder()
            .WithTitle("🚀 GitHub Push")
            .AddField("Repo", payload.Repo)
            .AddField("Pusher", payload.Pusher)
            .AddField("Commit", payload.Commit.Length >= 7 ? payload.Commit.Substring(0, 7) : payload.Commit)
            .AddField("Message", payload.Message)
            .WithColor(Color.Green)
            .WithTimestamp(DateTimeOffset.Now)
            .Build();

        await channel.SendMessageAsync(embed: embed);
    }
}