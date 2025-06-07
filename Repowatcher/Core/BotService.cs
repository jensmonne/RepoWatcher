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
        
        var unixTime = new DateTimeOffset(payload.HeadCommit.Timestamp).ToUnixTimeSeconds();
        var timeString = $"<t:{unixTime}:f>";

        var embed = new EmbedBuilder()
            .WithTitle("🚀 GitHub Push")
            .AddField("Repository", payload.Repository.FullName)
            .AddField("Pusher", payload.Pusher.Name)
            .AddField("Time", timeString)
            .AddField("Message", payload.HeadCommit.Message)
            .WithColor(Color.Green)
            .Build();

        var selectMenu = new SelectMenuBuilder()
            .WithCustomId("push_files_select")
            .WithPlaceholder("Select change type")
            .AddOption("Added", "added", description: $"{payload.HeadCommit.Added.Count} files added")
            .AddOption("Modified", "modified", description: $"{payload.HeadCommit.Modified.Count} files modified")
            .AddOption("Removed", "removed", description: $"{payload.HeadCommit.Removed.Count} files removed");
        
        var component = new ComponentBuilder()
            .WithSelectMenu(selectMenu)
            .Build();
        
        await channel.SendMessageAsync(embed: embed, components: component);
    }
}