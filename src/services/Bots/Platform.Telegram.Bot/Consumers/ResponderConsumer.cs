namespace Platform.Telegram.Bot.Consumers
{
    public class ResponderConsumer // : IConsumeAsync<TelegramProfile>
    {
        // private readonly ITelegramBotClient _botClient;
        // private readonly ILogger _logger;
        //
        // public ResponderConsumer(ITelegramBotClient botClient, ILogger<ResponderConsumer> logger)
        // {
        //     _botClient = botClient;
        //     _logger = logger;
        // }
        //
        // public async Task ConsumeAsync(TelegramProfile profile, CancellationToken cancellationToken = new())
        // {
        //     // await _botClient.SendChatActionAsync(profile.SessionContext.ChatId, ChatAction.Typing, cancellationToken);
        //     // await SendFileToChat(profile.SessionContext.ChatId, profile.FileBody, profile.FileName, cancellationToken);
        // }
        //
        // /// <summary>
        // /// Send file to chat
        // /// </summary>
        // private async Task SendFileToChat(long? chatId, byte[] body, string fileName,
        //     CancellationToken cancellationToken)
        // {
        //     await using var stream = new MemoryStream(body);
        //     var inputMedia = new InputMedia(stream, fileName);
        //     await _botClient.SendDocumentAsync(chatId, inputMedia, cancellationToken: cancellationToken);
        // }
    }
}