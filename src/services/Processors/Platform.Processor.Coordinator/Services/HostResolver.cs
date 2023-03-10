using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Platform.Processor.Coordinator.Services;

public class HostResolver : IHostResolver
{
    public Task<IPAddressCollection> Resolve(string name)
    {
        throw new NotImplementedException();
    }
}