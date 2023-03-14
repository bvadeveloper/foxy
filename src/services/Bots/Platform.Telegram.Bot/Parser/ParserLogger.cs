using System.CommandLine;
using System.CommandLine.IO;
using System.IO;
using System.Text;

namespace Platform.Telegram.Bot.Parser;

internal class ParserLogger : IConsole
{
    public ParserLogger()
    {
        Out = new StandardStreamWriter();
        Error = new StandardStreamWriter();
    }

    public IStandardStreamWriter Error { get; }

    public IStandardStreamWriter Out { get; }

    private string ErrorMessage => Error.ToString() ?? string.Empty;

    private string OutMessage => Out.ToString() ?? string.Empty;

    public bool IsOutputRedirected { get; protected set; }

    public bool IsErrorRedirected { get; protected set; }

    public bool IsInputRedirected { get; protected set; }

    internal string CollectOutput() =>
        string.IsNullOrEmpty(ErrorMessage)
            ? string.IsNullOrEmpty(OutMessage)
                ? UserMessages.ParseDefaultMessage
                : OutMessage
            : ErrorMessage;

    private class StandardStreamWriter : TextWriter, IStandardStreamWriter
    {
        private readonly StringBuilder _stringBuilder = new();

        public override void Write(char value) => _stringBuilder.Append(value);

        public override void Write(string? value) => _stringBuilder.Append(value);

        public override Encoding Encoding { get; } = Encoding.Unicode;

        public override string ToString() => _stringBuilder.ToString();
    }
}