using System.Threading.Tasks;

namespace Platform.Bus.Publisher
{
    public interface IPublisher
    {
        Task Publish(object payload);
    }
}