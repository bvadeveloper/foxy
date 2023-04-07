using System.Threading.Tasks;
using Platform.Contract.Profiles.Processors;

namespace Platform.Processor.Reporter.Clients;

public interface ITelegramClient
{
    ValueTask SendToTelegram(ReportProfile profile);
}