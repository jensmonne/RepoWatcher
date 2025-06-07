using System.Text.Json.Serialization;

namespace Repowatcher;

public class GitHubPushPayload
{
    [JsonPropertyName("repository")]
    public Repository Repository { get; set; }

    [JsonPropertyName("pusher")]
    public Pusher Pusher { get; set; }

    [JsonPropertyName("head_commit")]
    public Commit HeadCommit { get; set; }
}

public class Repository
{
    [JsonPropertyName("full_name")]
    public string FullName { get; set; }
}

public class Pusher
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class Commit
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}