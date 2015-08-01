using System.Net;
using System.Threading.Tasks;

namespace ScarletLock
{
    public interface IConnectionFactory
    {
        Task<IConnection> CreateAndConnectAsync(EndPoint[] endpoints);
    }
}