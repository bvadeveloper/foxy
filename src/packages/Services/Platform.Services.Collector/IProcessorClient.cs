using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Collectors;

namespace Platform.Services.Collector;

public interface IProcessorClient
{
    ValueTask SendToReporter(IProfile profile);
    
    ValueTask SendToCoordinatorSync(CollectorInfo collectorInfo, byte[] ipAddressBytes, byte[] publicKey);
}