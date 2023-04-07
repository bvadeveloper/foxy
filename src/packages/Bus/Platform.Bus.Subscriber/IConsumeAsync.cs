using System.Threading.Tasks;
using Platform.Contract.Profiles;

namespace Platform.Bus.Subscriber;

public interface IConsumeAsync<in T>  where T : IProfile
{
    Task ConsumeAsync(T profile);
}