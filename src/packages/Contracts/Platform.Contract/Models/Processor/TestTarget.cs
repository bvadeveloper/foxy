using Platform.Contract.Abstractions;

namespace Platform.Contract.Models.Processor
{
    // TODO: remove after testing
    public record TestTarget : ITarget, IProcessorExchange
    {
        public string Name { get; set; }
    }
}