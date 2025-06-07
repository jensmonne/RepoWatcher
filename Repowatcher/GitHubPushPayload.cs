namespace Repowatcher;

public class GitHubPushPayload
{
    public string Repo { get; set; }
    public string Pusher { get; set; }
    public string Commit { get; set; }
    public string Message { get; set; }
}