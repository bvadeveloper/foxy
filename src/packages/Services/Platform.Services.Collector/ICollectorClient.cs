using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Collectors;

namespace Platform.Services.Collector;

public interface ICollectorClient
{
    ValueTask SendToReporter(IProfile profile);
    
    ValueTask SendToCoordinatorSync(CollectorInfo collectorInfo, byte[] ipAddressBytes, byte[] publicKey);
}