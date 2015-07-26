using System;

namespace ScarletLock
{
    public interface IDistributedLock<TIdentity>
    {
        string Resource { get; }

        TIdentity Identity { get; }       

        TimeSpan Timeout { get; }
    }
}