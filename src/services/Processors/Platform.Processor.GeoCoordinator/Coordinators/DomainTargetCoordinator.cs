using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher;
using Platform.Contract.Abstractions;
using Platform.Contract.Enums;
using Platform.Contract.Models.Bot;

namespace Platform.Processor.GeoCoordinator.Coordinators
{
    public class DomainTargetCoordinator //: IConsumeAsync<DomainTarget>
    {
        private readonly IPublisher _publishClient;
        private readonly ILogger _logger;

        public DomainTargetCoordinator(
            IPublisher publishClient,
            ILogger<DomainTargetCoordinator> logger)
        {
            _publishClient = publishClient;
            _logger = logger;
        }

        public async Task ConsumeAsync(DomainTarget target, CancellationToken cancellationToken = new())
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