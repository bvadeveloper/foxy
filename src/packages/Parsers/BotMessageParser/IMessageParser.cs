using System.Threading;
using System.Threading.Tasks;

namespace BotMessageParser;

public interface IMessageParser
{
    ValueTask<ParseResult> Parse(string value, CancellationToken cancellationToken);
}