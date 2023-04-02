using Platform.Contract.Profiles;

namespace Platform.Services.Collector;

public interface ICollectorPublisher
{
    ValueTask PublishToReport(IProfile profile);
}