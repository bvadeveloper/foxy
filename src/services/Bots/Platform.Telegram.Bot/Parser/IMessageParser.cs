using System.Threading.Tasks;

namespace Platform.Telegram.Bot.Parser;

public interface IMessageParser
{
    Task<ParseResult> Parse(string value);
}