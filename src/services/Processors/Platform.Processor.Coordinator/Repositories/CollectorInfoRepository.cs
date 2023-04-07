using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Caching.Abstractions;
using Platform.Contract.Profiles.Enums;
using Platform.Contract.Profiles.Extensions;
using Platform.Cryptography;
using Platform.Geolocation.HostGeolocation;
using Platform.Logging.Extensions;
using Platform.Processor.Coordinator.Exceptions;

namespace Platform.Processor.Coordinator.Repositories;

public class CollectorInfoRepository : ICollectorInfoRepository
{
    private const int ScanCount = 2;

    private readonly IHostGeolocation _geolocation;
    private readonly ICacheDataService _cacheDataService;
    private readonly ILogger _logger;

    public CollectorInfoRepository(IHostGeolocation geolocation, ICacheDataService cacheDataService, ILogger<CollectorInfoRepository> logger)
    {
        _geolocation = geolocation;
        _cacheDataService = cacheDataService;
        _logger = logger;
    }

    public async ValueTask<(byte[] publicKey, string route)> FindAny(ProcessingTypes processingType) =>
        await FindInCache(processingType, MakeKey(processingType));

    public async ValueTask<(byte[] publicKey, string route)> FindByIp(ProcessingTypes processingType, IPAddress ipAddress)
    {
        var countryCode = await _geolocation.FindCountryCode(ipAddress);
        return await FindInCache(processingType, MakeKey(processingType, countryCode));
    }

    public async ValueTask<(byte[] publicKey, string route)> FindByIpOrAny(ProcessingTypes processingType, IPAddress ipAddress)
    {
        var countryCode = await _geolocation.FindCountryCode(ipAddress);
        return await FindInCache(processingType, MakeKey(processingType, countryCode), true);
    }

    public async ValueTask<(byte[] publicKey, string route)> FindByCountryCode(ProcessingTypes processingType, string countryCode) =>
        await FindInCache(processingType, MakeKey(processingType, countryCode));

    public async ValueTask<(byte[] publicKey, string route)> FindByCountryCodeOrAny(ProcessingTypes processingType, string countryCode) =>
        await FindInCache(processingType, MakeKey(processingType, countryCode), true);

    private async ValueTask<(byte[] publicKey, string route)> FindInCache(ProcessingTypes processingType, string key, bool any = false)
    {
        var routeKeys = await _cacheDataService.KeyScan(key, ScanCount);
        if (routeKeys.Any())
        {
            var routeKey = routeKeys.RandomAny();
            var routeValue = await _cacheDataService.GetValue(routeKey);
            var (publicKey, route) = routeValue.SplitValue();

            return (publicKey.ToBytesFromBase64(), route);
        }

        // during operation need to adjust for specific cases
        if (any)
        {
            var routeKeysAny = await _cacheDataService.KeyScan(MakeKey(processingType), ScanCount);
            if (routeKeysAny.Any())
            {
                var routeKey = routeKeysAny.RandomAny();
                var routeValue = await _cacheDataService.GetValue(routeKey);
                var (publicKey, route) = routeValue.SplitValue();

                return (publicKey.ToBytesFromBase64(), route);
            }
        }

        var exception = new NoCollectorException($"No collector routes found with type '{processingType}' and key '{key}'");
        _logger.Error($"Looks like we haven't any collectors", exception);

        throw exception;
    }

    private static string MakeKey(ProcessingTypes processingType, string countryCode = "*") =>
        $"{countryCode}:{processingType.ToLower()}:*";
}