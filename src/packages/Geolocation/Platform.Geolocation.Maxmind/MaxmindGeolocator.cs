using System.Net;
using MaxMind.Db;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using Microsoft.Extensions.Logging;
using Platform.Logging.Extensions;

namespace Platform.Geolocation.Maxmind;

public class MaxmindGeolocator : IGeolocator
{
    private readonly ILogger _logger;
    private readonly string _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "country.mmdb");

    public MaxmindGeolocator(ILogger<MaxmindGeolocator> logger) => _logger = logger;

    public ValueTask<string> FindCountryCode(IPAddress ipAddress)
    {
        var result = string.Empty;
        try
        {
            using var reader = new DatabaseReader(_dbPath, FileAccessMode.MemoryMapped); // todo: move reader to singleton
            var response = reader.Country(ipAddress);
            result = response.Country.IsoCode ?? string.Empty;
        }
        catch (InvalidDatabaseException invalidDatabaseException)
        {
            _logger.Error($"It looks like we've caught db '{_dbPath}' exception '{invalidDatabaseException.Message}'.", invalidDatabaseException);
        }
        catch (AddressNotFoundException notFoundException)
        {
            _logger.Error("Hmm, it looks like the address is not found, try to update your db.", notFoundException);
        }
        catch (Exception e)
        {
            _logger.Error($"{nameof(MaxmindGeolocator)} throw an exception '{e.Message}'", e);
        }

        return ValueTask.FromResult(result);
    }
}