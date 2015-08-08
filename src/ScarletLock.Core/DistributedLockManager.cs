using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Polly;
using ScarletLock.TinyIoC;

namespace ScarletLock
{
    public class DistributedLockManager<TIdentity> : IDistributedLockManager<TIdentity>
    {
        protected DistributedLockManager(IDistributedLockFactory<TIdentity> lockFactory,
            TimeSpan defaultTTL, Policy retryPolicy,
            params IConnection[] connections)
        {
            DistributedLockFactory = lockFactory;
            DefaultTTL = defaultTTL;
            RetryPolicy = retryPolicy;
            Connections = connections;
        }

        protected IConnection[] Connections { get; }

        protected Policy RetryPolicy { get; }

        protected IDistributedLockFactory<TIdentity> DistributedLockFactory { get; }

        protected int QuorumCount => Connections.Length == 1 ? 1 : (Connections.Length / 2) + 1;

        public TimeSpan DefaultTTL { get; }

        public async Task ReleaseDistributedLockAsync(IDistributedLock<TIdentity> distributedLock)
        {
            foreach (var connection in Connections)
                await UnlockOnInstanceAsync(connection, distributedLock);
        }

        public async Task<IDistributedLock<TIdentity>> AcquireDistributedLockAsync(string resource)
        {
            return await AcquireDistributedLockAsync(resource, DefaultTTL);
        }

        public async Task<IDistributedLock<TIdentity>> AcquireDistributedLockAsync(string resource, TimeSpan ttl)
        {
            var drift = TimeSpan.FromMilliseconds(Convert.ToInt32((ttl.TotalMilliseconds * 0.01) + 2));

            var preliminaryLock = DistributedLockFactory.GetPreliminaryLock(resource);

            return await RetryPolicy
                .ExecuteAsync(async () =>
                {
                    var startTime = DateTime.Now;

                    var locksAcquired = 0;
                    var temporaryLocks = new List<IConnection>();
                    foreach (var connection in Connections)
                        if (await AttemptLockOnInstanceAsync(connection, preliminaryLock, ttl))
                        {
                            temporaryLocks.Add(connection);
                            locksAcquired++;
                        }

                    var validityTime = ttl - (DateTime.Now - startTime) - drift;

                    if (locksAcquired >= QuorumCount && validityTime.TotalMilliseconds > 0)
                        return DistributedLockFactory.EstablishLock(this, preliminaryLock, DateTime.Now + validityTime);

                    //Locking was not successful, release our previous locks.
                    foreach (var connection in temporaryLocks)
                        await UnlockOnInstanceAsync(connection, preliminaryLock);

                    //We need to wait, throw so retry policy can execute.
                    throw new BlockedException();
                });
        }

        protected async Task<bool> AttemptLockOnInstanceAsync(IConnection connection,
            PreliminaryLock<TIdentity> preliminaryLock, TimeSpan ttl)
        {
            try
            {
                return await connection.SetStringWhenNotSetAsync(preliminaryLock.Resource, preliminaryLock.Identity.ToString(), ttl);
            }
            catch (Exception ex)
            {
                //TODO: Log ex

                return false;
            }
        }

        protected async Task UnlockOnInstanceAsync(IConnection connection,
            IDistributedLock<TIdentity> distributedLock)
        {
            await connection.DeleteIfMatchedAsync(distributedLock.Resource, distributedLock.Identity.ToString());
        }

        protected async Task UnlockOnInstanceAsync(IConnection connection, PreliminaryLock<TIdentity> preliminaryLock)
        {
            await connection.DeleteIfMatchedAsync(preliminaryLock.Resource, preliminaryLock.Identity.ToString());
        }

        internal static async Task<DistributedLockManager<TIdentity>> CreateAndConnectAsync(TimeSpan defaultTTL, params ServerDetails[] servers)
        {
            return await CreateAndConnectAsync(defaultTTL, -1, servers);
        }

        internal static async Task<DistributedLockManager<TIdentity>> CreateAndConnectAsync(TimeSpan defaultTTL,
            int retryAttempts, params ServerDetails[] servers)
        {
            var container = TinyIoCContainer.Current;

            var connectionFactory = container.Resolve<IConnectionFactory>();

            var connections = new IConnection[servers.Length];

            for (var i = 0; i < servers.Length; i++)
            {
                connections[i] = await connectionFactory.CreateAndConnectAsync(servers[i].EndPoints);
            }

            var retryPolicy = Policy.Handle<BlockedException>()
                .WaitAndRetryAsync(GenerateRandomWaitSequence(-1));

            return new DistributedLockManager<TIdentity>(container.Resolve<IDistributedLockFactory<TIdentity>>(), defaultTTL, retryPolicy, connections);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetType().FullName);

            sb.AppendLine("------Endpoints------");
            foreach (var connection in Connections)
            {
                sb.AppendLine(connection.Endpoints.First().ToString());
            }

            return sb.ToString();
        }

        private static IEnumerable<TimeSpan> GenerateRandomWaitSequence(int retries)
        {
            for (var i = 0; i != retries; i++)
            {
                if (i == int.MaxValue)
                    i = 0;

                yield return TimeSpan.FromMilliseconds(new Random().Next(10, 200));
            }
        }

    }
}