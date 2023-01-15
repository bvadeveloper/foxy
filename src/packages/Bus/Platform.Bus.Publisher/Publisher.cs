using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher.Abstractions;
using Platform.Contract.Abstractions;
using Platform.Logging.Extensions;
using Platform.Primitive;

namespace Platform.Bus.Publisher
{
    public class Publisher : IPublisher
    {
        private readonly IBus _bus;
        private readonly ILogger _logger;

        public Publisher(IBus bus, ILogger<Publisher> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        public async Task<Result<string>[]> Publish<T>(IEnumerable<T> targets) where T : ITarget =>
            await Task.WhenAll(targets.Select(Publish));

        public async Task<Result<string>> Publish<T>(T target) where T : ITarget
        {
            try
            {
                await _bus.PubSub.PublishAsync(target);
                return new Result<string>().UseResult($"{target.Value} - processing").Ok();
            }
            catch (Exception e)
            {
                _logger.Error($"An error was thrown while sending a request '{e.Message}'", e, ("session", target.SessionContext));
                return new Result<string>().UseResult($"{target.Value} - can't process, something went wrong").Fail();
            }
        }
    }
}