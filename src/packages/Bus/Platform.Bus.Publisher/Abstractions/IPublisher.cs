using System.Collections.Generic;
using System.Threading.Tasks;
using Platform.Contract.Abstractions;
using Platform.Primitive;

namespace Platform.Bus.Publisher.Abstractions
{
    public interface IPublisher
    {
        Task<Result<string>[]> Publish<TProfile>(IEnumerable<TProfile> profiles) where TProfile : ITarget;
        
        Task<Result<string>> Publish<TProfile>(TProfile profile) where TProfile : ITarget;
    }
}