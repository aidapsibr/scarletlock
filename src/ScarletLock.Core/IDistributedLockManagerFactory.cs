using System;
using System.Threading.Tasks;

namespace ScarletLock
{
    public interface IDistributedLockManagerFactory
    {
        Task<IDistributedLockManager<TIdentity>> CreateAndConnectAsync<TIdentity>(TimeSpan defaultTTL, params ServerDetails[] servers);
        Task<IDistributedLockManager<TIdentity>> CreateAndConnectAsync<TIdentity>(TimeSpan defaultTTL, int retryAttempts, params ServerDetails[] servers);
    }
}