using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher.Abstractions;
using Platform.Contract;
using Platform.Contract.Abstractions;
using Platform.Logging.Extensions;
using Platform.Primitive;

namespace Platform.Bus.Publisher
{
    public class PublishClient : IPublishClient
    {
        private readonly IBus _bus;
        private readonly ILogger _logger;

        public PublishClient(IBus bus, ILogger<PublishClient> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        public async Task<Result<string>[]> Publish<TProfile>(IEnumerable<TProfile> profiles) where TProfile : ITarget =>
            await Task.WhenAll(profiles.Select(Publish));

        public async Task<Result<string>> Publish<TProfile>(TProfile profiles) where TProfile : ITarget
        {
            try
            {
                await _bus.PubSub.PublishAsync(profiles);
                return new Result<string>().UseResult($"{profiles.Target} - processing").Ok();
            }
            catch (Exception e)
            {
                _logger.Error($"An error was thrown while sending a request '{e.Message}'", e);
                return new Result<string>().UseResult($"{profiles.Target} - can't process, something went wrong").Fail();
            }
        }
    }
}