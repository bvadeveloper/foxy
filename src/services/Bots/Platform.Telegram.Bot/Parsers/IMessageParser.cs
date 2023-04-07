using System.Threading.Tasks;

namespace Platform.Telegram.Bot.Parsers;

public interface IMessageParser
{
    ValueTask<ParseResult> Parse(string value);
}