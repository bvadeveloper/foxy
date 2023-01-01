using System.Collections.Generic;
using System.Threading.Tasks;
using Platform.Contract;
using Platform.Contract.Abstractions;
using Platform.Primitive;

namespace Platform.Bus.Publisher.Abstractions
{
    public interface IPublishClient
    {
        Task<Result<string>[]> Publish<TProfile>(IEnumerable<TProfile> targets) where TProfile : ITargetProfile;
        
        Task<Result<string>> Publish<TProfile>(TProfile profile) where TProfile : ITargetProfile;
    }
}