using System;

namespace ScarletLock
{
    public interface IDistributedLockFactory<TIdentity>
    {

        IDistributedLock<TIdentity> EstablishLock(PreliminaryLock<TIdentity> preliminaryLock , TimeSpan expiration);

        PreliminaryLock<TIdentity> GetPreliminaryLock(string resoure);
    }
}