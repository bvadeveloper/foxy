using Microsoft.Extensions.Hosting;
using Platform.Caching.Abstractions;
using Platform.Cryptography;

namespace Platform.Services.Processor
{
    public class CryptographicKeySynchronizationService : IHostedService
    {
        private const string CacheKey = "processor:keypair";

        private readonly ICacheDataService _cacheDataService;
        private readonly ICryptographicService _cryptographicService;

        public CryptographicKeySynchronizationService(ICacheDataService cacheDataService, ICryptographicService cryptographicService)
        {
            _cacheDataService = cacheDataService;
            _cryptographicService = cryptographicService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                var keyPair = await _cacheDataService.GetValue(CacheKey);
                if (string.IsNullOrEmpty(keyPair))
                {
                    await _cacheDataService.SetValue(CacheKey, _cryptographicService.GetKeyPair(), default, true);
                    break;
                }

                if (_cryptographicService.TrySetKeyPair(keyPair))
                {
                    break;
                }

                await _cacheDataService.Delete(CacheKey, true);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}