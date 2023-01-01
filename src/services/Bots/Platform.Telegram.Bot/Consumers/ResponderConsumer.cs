using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Platform.Contract.Telegram;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Platform.Telegram.Bot.Consumers
{
    public class ResponderConsumer : IConsumeAsync<TelegramProfile>
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        public ResponderConsumer(ITelegramBotClient botClient, ILogger<ResponderConsumer> logger)
        {
            _botClient = botClient;
            _logger = logger;
        }

        public async Task ConsumeAsync(TelegramProfile profile, CancellationToken cancellationToken = new())
        {
            await _botClient.SendChatActionAsync(profile.TraceContext.ChatId, ChatAction.Typing, cancellationToken);
            await SendFileToChat(profile.TraceContext.ChatId, profile.FileBody, profile.FileName, cancellationToken);
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