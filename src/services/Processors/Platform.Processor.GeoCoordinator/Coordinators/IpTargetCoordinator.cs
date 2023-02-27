using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher;
using Platform.Contract;
using Platform.Contract.Enums;
using Platform.Contract.Messages;
using Platform.Contract.Messages.Messages;

namespace Platform.Processor.GeoCoordinator.Coordinators
{
    public class IpTargetCoordinator // : IConsumeAsync<IpTarget>
    {
        private readonly IPublisher _publishClient;
        private readonly ILogger _logger;

        public IpTargetCoordinator(
            IPublisher publishClient,
            ILogger<IpTargetCoordinator> logger)
        {
            _publishClient = publishClient;
            _logger = logger;
        }

        public async Task ConsumeAsync(IpMessage target, CancellationToken cancellationToken = new())
        {
            await PublishScanProfile(target);
        }

        private async Task PublishScanProfile(ITarget profile, Dictionary<TargetType, List<string>> tags = default)
        {
            // await _publishClient.Publish(new DomainScanProfile
            // {
            //     SessionContext = profile.SessionContext,
            //     Value = profile.Value,
            //     Tags = tags
            // });
        }
    }
}