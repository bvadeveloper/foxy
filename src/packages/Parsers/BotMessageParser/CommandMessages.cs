namespace BotMessageParser;

/// <summary>
/// Provides constant messages for the Foxy command-line interface.
/// </summary>
internal static class CommandMessages
{
    internal const string RootCommandDescription = "Foxy command-line interface for Telegram \n\n" +
                                                             "Options: \n" +
                                                             "/help    Show help and usage information \n\n" +
                                                             "Usage: \n" +
                                                             "  [command] [options] \n\n" +
                                                             "Commands: \n" +
                                                             "/scan    Collect information about endpoints. \n" +
                                                             "  --domains, -d foxy.com \n" +
                                                             "  --hosts, -h 8.8.8.8 \n\n" +
                                                             "/search    Different searches for OSINT (under construction). \n" +
                                                             "  --emails, -e foxy@foxy.com \n" +
                                                             "  --bitcoins, -b 1234567890 \n\n" +
                                                             "/parse    Collect information about SMP (under construction).\n" +
                                                             "  --facebook, -f \n" +
                                                             "  --instagram, -i \n";
    
    internal const string DefaultMessage = "Suggestion for you mate try to use /help for more details.";
}