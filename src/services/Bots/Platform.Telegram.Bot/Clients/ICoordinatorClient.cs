using System.Threading.Tasks;
using Platform.Contract.Profiles.Processors;

namespace Platform.Telegram.Bot.Clients;

public interface ICoordinatorClient
{
    ValueTask SendToCoordinator(CoordinatorProfile profile);
}