using System.Threading.Tasks;
using Platform.Contract;
using Platform.Contract.Abstractions;
using Platform.Primitive;

namespace Platform.Bus.Publisher.Abstractions
{
    public interface IPublisher
    {
        Task<Result<string>> Publish(ITarget message);
    }
}