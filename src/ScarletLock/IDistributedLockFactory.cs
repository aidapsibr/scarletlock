using System;

namespace ScarletLock
{
    public interface IDistributedLockFactory<TIdentity>
    {
        IDistributedLock<TIdentity> EstablishLock(string resource);

        IDistributedLock<TIdentity> EstablishLock(string resource, TimeSpan lockTimeoutDelay);
    }
}