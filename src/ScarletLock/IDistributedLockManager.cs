using System;
using System.Threading.Tasks;

namespace ScarletLock
{
    public interface IDistributedLockManager<TIdentity>
    {
        TimeSpan DefaultTTL { get; }
        Task ReleaseDistributedLockAsync(IDistributedLock<TIdentity> distributedLock);
        Task<IDistributedLock<TIdentity>> AcquireDistributedLockAsync(string resource);
        Task<IDistributedLock<TIdentity>> AcquireDistributedLockAsync(string resource, TimeSpan TTL);
    }
}