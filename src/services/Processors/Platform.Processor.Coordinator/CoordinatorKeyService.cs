using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Platform.Caching.Abstractions;
using Platform.Cryptography;

namespace Platform.Processor.Coordinator
{
    public class CoordinatorKeyService : IHostedService
    {
        private const string CacheKey = "processor:keypair";

        private readonly ICacheDataService _cacheDataService;
        private readonly DiffieHellmanKeyMaker _diffieHellmanKeyMaker;

        public CoordinatorKeyService(ICacheDataService cacheDataService, DiffieHellmanKeyMaker diffieHellmanKeyMaker)
        {
            _cacheDataService = cacheDataService;
            _diffieHellmanKeyMaker = diffieHellmanKeyMaker;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                var keyPair = await _cacheDataService.GetValue(CacheKey);
                if (string.IsNullOrEmpty(keyPair))
                {
                    await _cacheDataService.SetValue(CacheKey, _diffieHellmanKeyMaker.KeyPairBase64, default, true);
                    break;
                }

                if (_diffieHellmanKeyMaker.TrySetKeyPair(keyPair))
                {
                    break;
                }

                await _cacheDataService.Delete(CacheKey, true);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}