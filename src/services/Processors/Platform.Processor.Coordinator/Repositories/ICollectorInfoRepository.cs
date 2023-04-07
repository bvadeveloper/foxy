using System.Net;
using System.Threading.Tasks;
using Platform.Contract.Profiles.Enums;

namespace Platform.Processor.Coordinator.Repositories;

public interface ICollectorInfoRepository
{
    ValueTask<(byte[] publicKey, string route)> FindAny(ProcessingTypes processingType);
    ValueTask<(byte[] publicKey, string route)> FindByIp(ProcessingTypes processingType, IPAddress ipAddress);
    ValueTask<(byte[] publicKey, string route)> FindByIpOrAny(ProcessingTypes processingType, IPAddress ipAddress);
    ValueTask<(byte[] publicKey, string route)> FindByCountryCode(ProcessingTypes processingType, string countryCode);
    ValueTask<(byte[] publicKey, string route)> FindByCountryCodeOrAny(ProcessingTypes processingType, string countryCode);
}