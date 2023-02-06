using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher;
using Platform.Bus.Publisher.Abstractions;
using Platform.Contract;
using Platform.Contract.Abstractions;
using Platform.Contract.Enums;
using Platform.Contract.Models;
using Platform.Contract.Models.Bot;
using Platform.Contract.Scanner;

namespace Platform.Processor.GeoCoordinator.Coordinators
{
    public class IpTargetCoordinator : IConsumeAsync<IpTarget>
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

        public async Task ConsumeAsync(IpTarget target, CancellationToken cancellationToken = new())
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