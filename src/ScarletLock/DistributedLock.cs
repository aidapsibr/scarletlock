using System;

namespace ScarletLock
{
    public class DistributedLock<TIdentity> 
        : IDistributedLock<TIdentity>
    {
        protected DistributedLock(string resource, TIdentity identity, TimeSpan expiration)
        {
            Resource = resource;
            Identity = identity;
            Expiration = expiration;
        }

        public string Resource { get; }

        public TIdentity Identity { get; }

        public TimeSpan Expiration { get; }

        internal static IDistributedLock<TIdentity> EstablishLock(string resource, TIdentity identity, TimeSpan expiration)
        {
            return new DistributedLock<TIdentity>(resource, identity, expiration);
        }
    }
}