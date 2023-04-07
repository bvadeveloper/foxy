using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Platform.Caching.Abstractions;
using Platform.Cryptography;
using Platform.Logging.Extensions;

namespace Platform.Services.Processor
{
    public class CryptographicKeySynchronizationService : IHostedService
    {
        private const string CacheKey = "processor:keypair";

        private readonly ICacheDataService _cacheDataService;
        private readonly ICryptographicService _cryptographicService;
        private readonly ILogger _logger;

        public CryptographicKeySynchronizationService(ICacheDataService cacheDataService, ICryptographicService cryptographicService, ILogger<CryptographicKeySynchronizationService> logger)
        {
            _cacheDataService = cacheDataService;
            _cryptographicService = cryptographicService;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            while (true)
            {
                var keyPair = await _cacheDataService.GetValue(CacheKey);
                if (string.IsNullOrEmpty(keyPair))
                {
                    _logger.Trace("Set new key pairs to distributed cache.");
                    await _cacheDataService.SetValue(CacheKey, _cryptographicService.GetKeyPair(), default, true);
                    break;
                }

                if (_cryptographicService.TrySetKeyPair(keyPair))
                {
                    _logger.Trace("Key pairs from the distributed cache are set to host.");
                    break;
                }

                _logger.Trace("Key pairs are broken, let's try again.");
                await _cacheDataService.Delete(CacheKey, true);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}