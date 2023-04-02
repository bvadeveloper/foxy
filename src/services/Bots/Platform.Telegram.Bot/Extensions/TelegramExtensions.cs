using System.Threading;
using System.Threading.Tasks;
using Platform.Primitives;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Platform.Telegram.Bot.Extensions;

public static class TelegramExtensions
{
    internal static SessionContext AddChatId(this SessionContext context, long id)
    {
        context.ChatId = id.ToString();
        return context;
    }

    public static string MakeUserKey(this User? user) => $"{user.FirstName}:{user.Id}";

    internal static async Task Say(this ITelegramBotClient botClient, Chat chat, string message,
        CancellationToken token)
    {
        await botClient.SendChatActionAsync(chat, ChatAction.Typing, cancellationToken: token);
        await botClient.SendTextMessageAsync(chat, message, cancellationToken: token);
    }

    internal static bool IsAny(this string[]? values) =>
        values switch
        {
            null => false,
            _ => values.Length switch
            {
                0 => false,
                _ => true
            }
        };
}