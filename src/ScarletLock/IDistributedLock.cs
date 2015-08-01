using System;

namespace ScarletLock
{
    public interface IDistributedLock<out TIdentity>
        : IDisposable
    {
        string Resource { get; }
        TIdentity Identity { get; }
        DateTime Expiration { get; }
        bool IsReleased { get; }
        bool IsExpired { get; }
        void Release();
    }
}