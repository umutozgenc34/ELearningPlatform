namespace Core.Options;

public class ConnectionStringOption
{
    public const string Key = "ConnectionStrings";
    public string SqlCon { get; set; } = default!;
}
