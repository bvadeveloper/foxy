using MemoryPack;
using Platform.Contract.Messages.Routes;

namespace Platform.Contract.Messages.Messages
{
    [MemoryPackable]
    // TODO: remove after testing
    public partial record TestMessage : ITarget, IProcessorRoute
    {
        public string Name { get; set; }
    }
}