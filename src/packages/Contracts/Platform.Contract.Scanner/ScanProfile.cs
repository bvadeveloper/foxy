using Platform.Contract.Abstractions;
using Platform.Contract.Messages;

namespace Platform.Contract.Scanner
{
    public abstract class ScanProfile : ITarget, IToolProfile
    {
        public string Name { get; set; }

        public string[] Tools { get; set; }
        
    }
}