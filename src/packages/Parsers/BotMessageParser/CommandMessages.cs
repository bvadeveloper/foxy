namespace BotMessageParser;

/// <summary>
/// Provides constant messages for the Foxy command-line interface.
/// </summary>
internal static class CommandMessages
{
    internal const string RootCommandDescription = "Foxy command-line interface for Telegram";

    internal const string DomainOptionDescription = "Collect information about domains.";
    internal const string HostOptionDescription = "Collect information about hosts.";
    internal const string EmailOptionDescription = "Collect information about emails.";
    internal const string BitcoinOptionDescription = "Collect information about bitcoins.";
    internal const string FacebookOptionDescription = "Collect information about Facebook user profiles.";
    internal const string InstagramOptionDescription = "Collect information about Instagram user profiles.";
    internal const string ExtraOptionsOptionDescription = "Additional arguments for the command (optional in some special cases).";

    internal const string ScanCommandDescription = "Collect information about endpoints like domains, IP addresses, etc.";
    internal const string SearchCommandDescription = "Different search commands for OSINT (under construction).";
    internal const string ParseCommandDescription = "Collect information from social media profiles (under construction).";
    
    internal const string ParseDefaultMessage = "A suggestion. Try using --help, it may help you.";
}