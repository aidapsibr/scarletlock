using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScarletLock.TinyIoC;

namespace ScarletLock
{
    public sealed class ScarletLock
    {
        private ScarletLock()
        {
            var container = TinyIoCContainer.Current;

            container
                .Register((kernel, overloads) =>
                    DistributedLockFactory<Guid>.Create(Guid.NewGuid));

            container
                .Register((kernel, overloads) =>
                    DistributedLockManagerFactory.Create());
        }

        private static ScarletLock _Instance;

        private static readonly object SyncRoot = new object();

        private static TinyIoCContainer Container => TinyIoCContainer.Current;


        public static void RegisterDistributedLockFactory<TIdentity>(Func<IDistributedLockFactory<TIdentity>> builderFunc)
        {
            Container
                .Register((kernel, overloads) =>
                    builderFunc());
        }

        public static void RegisterConnectionFactory(Func<IConnectionFactory> builderFunc)
        {
            Container
                .Register((kernel, overloads) =>
                    builderFunc());
        }

        public async Task<IDistributedLockManager<TIdentity>> BuildDistributedLockManager<TIdentity>(TimeSpan defaultTTL,
            int retryAttempts, params ServerDetails[] servers)
        {
            return await Container
                .Resolve<IDistributedLockManagerFactory>()
                .CreateAndConnectAsync<TIdentity>(defaultTTL, retryAttempts, servers);
        }

        public async Task<IDistributedLockManager<TIdentity>> BuildDistributedLockManager<TIdentity>(TimeSpan defaultTTL,
            params ServerDetails[] servers)
        {
            return await Container
                .Resolve<IDistributedLockManagerFactory>()
                .CreateAndConnectAsync<TIdentity>(defaultTTL, servers);
        }

        public static ScarletLock Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (SyncRoot)
                    {
                        if(_Instance == null)
                            _Instance = new ScarletLock();
                    }
                }

                return _Instance;
            }
        }
    }
}
