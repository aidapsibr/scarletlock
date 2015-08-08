using System;

namespace ScarletLock
{
    public class DistributedLock<TIdentity> 
        : IDistributedLock<TIdentity>
    {
        private readonly IDistributedLockManager<TIdentity> _DistributedLockManager; 
        protected DistributedLock(IDistributedLockManager<TIdentity> dlm, string resource, TIdentity identity, DateTime expiration)
        {
            _DistributedLockManager = dlm;
            Resource = resource;
            Identity = identity;
            Expiration = expiration;
        }

        public string Resource { get; }

        public TIdentity Identity { get; }

        public DateTime Expiration { get; }

        public bool IsReleased { get; private set; }

        public bool IsExpired => DateTime.Now >= Expiration;

        public void Release()
        {
            if (!IsReleased && !IsExpired)
            {
                _DistributedLockManager?.ReleaseDistributedLockAsync(this).Wait();
                IsReleased = true;
            }
        }

        internal static IDistributedLock<TIdentity> EstablishLock(IDistributedLockManager<TIdentity> dlm, string resource, TIdentity identity, DateTime expiration)
        {
            return new DistributedLock<TIdentity>(dlm, resource, identity, expiration);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                Release();
            }
        }
    }
}