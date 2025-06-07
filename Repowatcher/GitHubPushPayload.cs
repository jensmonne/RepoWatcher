using System.Text.Json.Serialization;

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

    [JsonPropertyName("description")]
    public string Description { get; set; }
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

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("added")]
    public List<string> Added { get; set; } = new();

    [JsonPropertyName("removed")]
    public List<string> Removed { get; set; } = new();

    [JsonPropertyName("modified")]
    public List<string> Modified { get; set; } = new();
}