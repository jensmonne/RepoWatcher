using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Repowatcher.Core;

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
            Client.SetGameAsync("your Repository", type: ActivityType.Watching);
            return Task.CompletedTask;
        };
        
        Client.SelectMenuExecuted += async (selectMenu) =>
        {
            if (selectMenu.Data.CustomId != "push_files_select")
                return;

            if (!_payloadCache.TryGetValue(selectMenu.Message.Id, out var payload))
            {
                await selectMenu.RespondAsync("Sorry, context expired or unavailable.", ephemeral: true);
                return;
            }

            string response = selectMenu.Data.Values.ElementAt(0) switch
            {
                "added" => FormatFileList("Added files", payload.HeadCommit.Added),
                "modified" => FormatFileList("Modified files", payload.HeadCommit.Modified),
                "removed" => FormatFileList("Removed files", payload.HeadCommit.Removed),
                _ => "Unknown option"
            };

            await selectMenu.RespondAsync(response, ephemeral: true);
        };
    }
    
    private string FormatFileList(string title, IReadOnlyCollection<string> files)
    {
        if (files == null || files.Count == 0)
            return $"{title}: None";

        return $"{title}:\n" + string.Join("\n", files);
    }
    
    private readonly Dictionary<ulong, GitHubPushPayload> _payloadCache = new();

    public async Task HandlePushEventAsync(GitHubPushPayload payload)
    {
        var channelIdString = DotNetEnv.Env.GetString("CHANNEL_ID");

        if (!ulong.TryParse(channelIdString, out ulong channelId))
        {
            Console.WriteLine("Invalid CHANNEL_ID in .env file");
            return;
        }
        
        var channel = Client.GetChannel(channelId) as IMessageChannel;
        if (channel == null) return;
        var unixTime = new DateTimeOffset(payload.HeadCommit.Timestamp).ToUnixTimeSeconds();
        var timeString = $"<t:{unixTime}:f>";

        var embed = new EmbedBuilder()
            .WithTitle("GitHub Push 🫸")
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
        
        var sentMessage = await channel.SendMessageAsync(embed: embed, components: component);
        
        _payloadCache[sentMessage.Id] = payload;
    }
}