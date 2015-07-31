using System;

namespace ScarletLock
{
    public interface IDistributedLock<out TIdentity>
    {
        string Resource { get; }

        TIdentity Identity { get; }       

        TimeSpan Expiration { get; }
    }
}