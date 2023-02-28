using System.Threading.Tasks;
using MemoryPack;
using Platform.Contract.Profiles;
using Platform.Primitives;

namespace Platform.Bus.Publisher;

public static class Extensions
{
    public static Task PublishToCoordinator(this IPublisher publisher, string message, SessionContext sessionContext)
    {
        var profile = new Profile(sessionContext, message);
        
        var bin = MemoryPackSerializer.Serialize(profile);
        var val = MemoryPackSerializer.Deserialize<Profile>(bin);

        // publisher.Publish();
        return Task.CompletedTask;
    }
}