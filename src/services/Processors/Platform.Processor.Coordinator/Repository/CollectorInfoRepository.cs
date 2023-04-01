using System;
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

namespace Platform.Processor.Coordinator.Repository;

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

    public async ValueTask<(byte[] publicKey, string route)> FindByCountryCode(ProcessingTypes processingType, string countryCode) =>
        await FindInCache(processingType, MakeKey(processingType, countryCode));

    private async ValueTask<(byte[] publicKey, string route)> FindInCache(ProcessingTypes processingType, string key)
    {
        var routeKeys = await _cacheDataService.KeyScan(key, ScanCount);
        if (routeKeys.Any())
        {
            var routeKey = routeKeys.Random();
            var routeValue = await _cacheDataService.GetValue(routeKey);
            var (collectorPublicKey, collectorRoute) = routeValue.SplitValue();

            return (collectorPublicKey.ToBytesFromBase64(), collectorRoute);
        }

        _logger.Error($"Looks like we haven't any collector routes with type '{processingType}'");

        throw new InvalidOperationException($"No collector routes with type '{processingType}'");
    }

    private static string MakeKey(ProcessingTypes processingType, string countryCode = "*") =>
        $"{countryCode}:{processingType.ToLower()}:*";
}