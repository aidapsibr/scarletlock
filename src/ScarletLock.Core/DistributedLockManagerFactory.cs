using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScarletLock
{
    public class DistributedLockManagerFactory : IDistributedLockManagerFactory
    {
        protected DistributedLockManagerFactory()
        {
            
        }

        public async Task<IDistributedLockManager<TIdentity>> CreateAndConnectAsync<TIdentity>(TimeSpan defaultTTL,
            int retryAttempts, params ServerDetails[] servers)
        {
            return await DistributedLockManager<TIdentity>.CreateAndConnectAsync(defaultTTL, retryAttempts, servers);
        }

        public async Task<IDistributedLockManager<TIdentity>> CreateAndConnectAsync<TIdentity>(TimeSpan defaultTTL, params ServerDetails[] servers)
        {
            return await DistributedLockManager<TIdentity>.CreateAndConnectAsync(defaultTTL, servers);
        }

        public static IDistributedLockManagerFactory Create()
        {
            return new DistributedLockManagerFactory();
        }
    }
}
