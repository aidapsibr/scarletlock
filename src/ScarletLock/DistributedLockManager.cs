using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScarletLock.TinyIoC;

namespace ScarletLock
{
    public class DistributedLockManager<TIdentity> : IDistributedLockManager<TIdentity>
    {
        protected DistributedLockManager(IDistributedLockFactory<TIdentity> lockFactory,
            TimeSpan defaultTTL,
            params IConnection[] connections)
        {
            DistributedLockFactory = lockFactory;
            DefaultTTL = defaultTTL;
            Connections = connections;
        }

        protected IConnection[] Connections { get; }

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

        public async Task<IDistributedLock<TIdentity>> AcquireDistributedLockAsync(string resource, TimeSpan TTL)
        {
            var drift = TimeSpan.FromMilliseconds(Convert.ToInt32((TTL.TotalMilliseconds * 0.01) + 2));

            var preliminaryLock = DistributedLockFactory.GetPreliminaryLock(resource);

            var startTime = DateTime.Now;

            var locksAcquired = 0;
            foreach (var connection in Connections)
            {
                if (await AttemptLockOnInstanceAsync(connection, preliminaryLock, TTL)) locksAcquired++;
            }

            var validityTime = TTL - (DateTime.Now - startTime) - drift;

            if (locksAcquired >= QuorumCount && validityTime.TotalMilliseconds > 0)
                return DistributedLockFactory.EstablishLock(this, preliminaryLock, DateTime.Now + validityTime);

            return null;
        }

        protected async Task<bool> AttemptLockOnInstanceAsync(IConnection connection,
            PreliminaryLock<TIdentity> preliminaryLock, TimeSpan TTL)
        {
            try
            {
                return await connection.SetStringWhenNotSetAsync(preliminaryLock.Resource, preliminaryLock.Identity.ToString(), TTL);
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

        public static async Task<DistributedLockManager<TIdentity>> CreateAndConnectAsync(TimeSpan defaultTTL, params ServerDetails[] servers)
        {
            var container = TinyIoCContainer.Current;

            var connectionFactory = container.Resolve<IConnectionFactory>();

            var connections = new IConnection[servers.Length];

            for (var i = 0; i < servers.Length; i++)
            {
                connections[i] = await connectionFactory.CreateAndConnectAsync(servers[i].EndPoints);
            }

            return new DistributedLockManager<TIdentity>(container.Resolve<IDistributedLockFactory<TIdentity>>(), defaultTTL, connections);
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
    }
}