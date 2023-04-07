namespace Platform.Bus;

public static class RoutNames
{
    public const string DefaultRoute = "default";
}

public static class HeaderNames
{
    public const string Session = "fx-session";
    public const string Iv = "fx-iv";
    public const string Key = "fx-key";
}

public static class ExchangeNames
{
    // bots
    public const string Telegram = "ex-telegram";
    
    // processors
    public const string Coordinator = "ex-coordinator";
    public const string Sync = "ex-sync";
    public const string Report = "ex-report";
    
    // collectors
    public const string Domain = "ex-domains";
    public const string Host = "ex-hosts";
    public const string Facebook = "ex-facebook";
    public const string Instagram = "ex-instagram";
    public const string Email = "ex-email";
}