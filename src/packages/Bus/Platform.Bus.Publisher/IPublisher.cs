using System.Threading.Tasks;
using Platform.Contract.Abstractions;
using Platform.Contract.Models;
using Platform.Primitive;

namespace Platform.Bus.Publisher
{
    public interface IPublisher
    {
        Task<Result<string>> Publish<T>(Message<T> message) where T : ITarget;
    }
}