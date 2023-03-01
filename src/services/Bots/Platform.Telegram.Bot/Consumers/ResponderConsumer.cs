using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Platform.Telegram.Bot.Consumers
{
    public class ResponderConsumer : IConsumeAsync<Profile>
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        public ResponderConsumer(ITelegramBotClient botClient, ILogger<ResponderConsumer> logger)
        {
            _botClient = botClient;
            _logger = logger;
        }


        public async ValueTask ConsumeAsync(Profile profile)
        {
            // await _botClient.SendChatActionAsync(profile.SessionContext.ChatId, ChatAction.Typing, cancellationToken);
            // await SendFileToChat(profile.SessionContext.ChatId, profile.FileBody, profile.FileName, cancellationToken);
        }

        /// <summary>
        /// Send file to chat
        /// </summary>
        private async Task SendFileToChat(long? chatId, byte[] body, string fileName,
            CancellationToken cancellationToken)
        {
            await using var stream = new MemoryStream(body);
            var inputMedia = new InputMedia(stream, fileName);
            await _botClient.SendDocumentAsync(chatId, inputMedia, cancellationToken: cancellationToken);
        }
    }
}