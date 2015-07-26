using System;

namespace ScarletLock
{
    public class DistributedLock<TIdentity> 
        : IDistributedLock<TIdentity>
    {
        protected DistributedLock(string resource, TIdentity identity, TimeSpan timeout)
        {
            Resource = resource;
            Identity = identity;
            Timeout = timeout;
        }

        public string Resource { get; }

        public TIdentity Identity { get; }

        public TimeSpan Timeout { get; }

        internal static IDistributedLock<TIdentity> EstablishLock(string resource, TIdentity identity, TimeSpan timeout)
        {
            return new DistributedLock<TIdentity>(resource, identity, timeout);
        }
    }
}