using System.Net;
using System.Threading.Tasks;

namespace ScarletLock.Redis.IntegrationTests.Mocks
{
    public class MockConnectionFactory 
        : IConnectionFactory
    {
        public async Task<IConnection> CreateAndConnectAsync(EndPoint[] endpoints)
        {
            return await Task.FromResult(new MockConnection(endpoints));
        }
    }
}
