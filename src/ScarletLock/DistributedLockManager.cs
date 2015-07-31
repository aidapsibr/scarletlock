using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ScarletLock
{
    public class DistributedLockManager<TIdentity>
    {
        protected ConnectionMultiplexer[] Connections { get; }

        protected IDistributedLockFactory<TIdentity> DistributedLockFactory { get; }

        public TimeSpan DefaultTTL { get; }

        protected DistributedLockManager(IDistributedLockFactory<TIdentity> lockFactory,
            TimeSpan defaultTTL,
            params ConnectionMultiplexer[] connections)
        {
            DistributedLockFactory = lockFactory;
            DefaultTTL = defaultTTL;
            Connections = connections;
        }

        public IDistributedLock<TIdentity> AcquireDistributedLockAsync(string resource)
        {
            var drift = TimeSpan.FromMilliseconds(Convert.ToInt32((DefaultTTL.TotalMilliseconds * 0.01) + 2));

            var preliminaryLock = DistributedLockFactory.GetPreliminaryLock(resource);
            DateTime startTime = DateTime.Now;

            var success = Connections[0].GetDatabase().StringSet(resource, preliminaryLock.Identity.ToString(), DefaultTTL, When.NotExists);

            return DistributedLockFactory.EstablishLock(preliminaryLock, DefaultTTL - (DateTime.Now - startTime) - drift);
        }

        public static async Task<DistributedLockManager<TIdentity>> CreateAndConnectAsync(
            Func<TIdentity> identityGeneratorFunc, TimeSpan defaultTTL, params ServerDetails[] servers)
        {
            var connections = new ConnectionMultiplexer[servers.Length];

            for (int i = 0; i < servers.Length; i++)
            {
                var configuration = new ConfigurationOptions { ClientName = "ScarletLock" };
                foreach (var endpoint in servers[i].EndPoints)
                    configuration.EndPoints.Add(endpoint);

                connections[i] = await ConnectionMultiplexer.ConnectAsync(configuration);
            }

            return
                new DistributedLockManager<TIdentity>(DistributedLockFactory<TIdentity>.Create(identityGeneratorFunc), defaultTTL, connections);
        }
    }
}