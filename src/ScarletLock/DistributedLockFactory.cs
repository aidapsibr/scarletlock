using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScarletLock
{
    public class DistributedLockFactory<TIdentity> 
        : IDistributedLockFactory<TIdentity>
    {
        protected Func<TIdentity> IdentityGenerator { get; }

        protected TimeSpan DefaultLockTimeoutDelay { get; }

        protected DistributedLockFactory(Func<TIdentity> identityGenerator, TimeSpan defaultLockTimeoutDelay)
        {
            IdentityGenerator = identityGenerator;
            DefaultLockTimeoutDelay = defaultLockTimeoutDelay;
        }

        public virtual IDistributedLock<TIdentity> EstablishLock(string resource)
        {
            return DistributedLock<TIdentity>.EstablishLock(resource, GenerateIdentity(), DefaultLockTimeoutDelay);
        }

        public virtual IDistributedLock<TIdentity> EstablishLock(string resource, TimeSpan lockTimeoutDelay)
        {
            return DistributedLock<TIdentity>.EstablishLock(resource, GenerateIdentity(), lockTimeoutDelay);
        }

        protected virtual TIdentity GenerateIdentity()
        {
            TIdentity identity = default(TIdentity);

            try
            {
                identity = IdentityGenerator.Invoke();
            }
            catch (Exception ex)
            {
                throw new IdentityGenerationException("An exception occured in identity generator", ex);
            }

            if (identity.Equals(default(TIdentity)))
                throw new IdentityGenerationException("Identity generatator provided default value.");

            return identity;
        }

        internal static IDistributedLockFactory<TIdentity> Create(Func<TIdentity> identityGenerator, TimeSpan defaultLockTimeoutDelay)
        {
            return new DistributedLockFactory<TIdentity>(identityGenerator, defaultLockTimeoutDelay);
        }
    }
}
