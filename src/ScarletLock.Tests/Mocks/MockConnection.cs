using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ScarletLock.Tests.Mocks
{
    public class MockConnection
        : IConnection
    {
        public MockConnection(EndPoint[] endpoints)
        {
            Endpoints = endpoints;
        }

        public EndPoint[] Endpoints { get; }

        public async Task<bool> SetStringWhenNotSetAsync(string resource, string identity, TimeSpan TTL)
        {
            return await Task.FromResult(true);
        }

        public Task DeleteIfMatchedAsync(string resource, string identity)
        {
            return Task.WhenAll(Enumerable.Empty<Task>());
        }

        public void Close()
        {
            
        }
    }
}
