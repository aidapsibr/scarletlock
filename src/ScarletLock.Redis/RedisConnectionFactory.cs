using System.Net;
using System.Threading.Tasks;

namespace ScarletLock.Redis
{
    public class RedisConnectionFactory : IConnectionFactory
    {
        protected RedisConnectionFactory()
        {
        }

        public static IConnectionFactory BuildConnectionFactory()
        {
            return new RedisConnectionFactory();
        }

        public async virtual Task<IConnection> CreateAndConnectAsync(EndPoint[] endpoints)
        {
            return await RedisConnection.ConnectAsync(endpoints);
        }
    }
}
