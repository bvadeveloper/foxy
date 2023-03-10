using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Platform.Processor.Coordinator.Services;

public interface IHostResolver
{
    Task<IPAddressCollection> Resolve(string name);
}