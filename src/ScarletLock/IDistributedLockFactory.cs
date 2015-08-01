using System;

namespace ScarletLock
{
    public interface IDistributedLockFactory<TIdentity>
    {

        IDistributedLock<TIdentity> EstablishLock(IDistributedLockManager<TIdentity> dlm, PreliminaryLock<TIdentity> preliminaryLock , DateTime expiration);

        PreliminaryLock<TIdentity> GetPreliminaryLock(string resoure);
    }
}