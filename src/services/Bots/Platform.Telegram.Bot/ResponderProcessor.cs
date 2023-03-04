using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Logging.Extensions;
using Platform.Primitives;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Platform.Telegram.Bot
{
    public class ResponderProcessor : IConsumeAsync<Profile>
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        private readonly SessionContext _sessionContext;


        public ResponderProcessor(ITelegramBotClient botClient, SessionContext sessionContext, ILogger<ResponderProcessor> logger)
        {
            _botClient = botClient;
            _sessionContext = sessionContext;
            _logger = logger;
        }


        public async ValueTask ConsumeAsync(Profile profile)
        {
            await _botClient.SendChatActionAsync(_sessionContext.ChatId, ChatAction.Typing);
            await SendFile(_sessionContext.ChatId, profile.FileReport.FileBody, profile.FileReport.FileName);
            _logger.Trace($"The file with name '{profile.FileReport.FileName}' was sent.");
        }

        /// <summary>
        /// Send file to chat
        /// </summary>
        private async Task SendFile(string chatId, byte[] body, string fileName)
        {
            await using var stream = new MemoryStream(body);
            var value = new InputMedia(stream, fileName);
            await _botClient.SendDocumentAsync(chatId, value);
        }
    }
}