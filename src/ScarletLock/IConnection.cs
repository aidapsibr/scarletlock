using System;
using System.Net;
using System.Threading.Tasks;

namespace ScarletLock
{
    public interface IConnection
    {
        EndPoint[] Endpoints { get; }

        Task<bool> SetStringWhenNotSetAsync(string resource, string identity, TimeSpan TTL);

        Task DeleteIfMatchedAsync(string resource, string identity);

        void Close();
    }
}