using System.Threading.Tasks;
using Platform.Contract.Profiles.Collectors;

namespace Platform.Processor.Coordinator.Clients;

public interface ICollectorClient
{
    ValueTask SendToDomainScanner(DomainProfile profile);
    ValueTask SendToHostScanner(HostProfile profile);
    ValueTask SendToEmailParser(EmailProfile profile);
    ValueTask SendToFacebookParser(FacebookProfile profile);
}