using System;
using System.Net;
using Xunit;

namespace ScarletLock.Redis.IntegrationTests
{
    public class DistributedLockManagerTests
    {
        public DistributedLockManagerTests()
        {
            ScarletLock.Instance
                .UseRedis();

        }

        [Fact(DisplayName = "DLM:ReleaseDistributedLock")]
        public void ReleaseDistributedLockAsyncTest()
        {
            var dlm = ScarletLock.Instance
                .BuildDistributedLockManager<Guid>(TimeSpan.FromSeconds(10),
                new ServerDetails { EndPoints = new EndPoint[] { new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6379) } }).Result;

            var distibutedLock = dlm.AcquireDistributedLockAsync("DLM:ReleaseDistributedLock").Result;

            dlm.ReleaseDistributedLockAsync(distibutedLock).Wait();
        }

        [Fact(DisplayName = "DLM:AcquireDistributedLock")]
        public void AcquireDistributedLockAsyncTest()
        {
            var dlm = ScarletLock.Instance
                .BuildDistributedLockManager<Guid>(TimeSpan.FromSeconds(10),
                new ServerDetails { EndPoints = new EndPoint[] { new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6379) } }).Result;

            var distibutedLock = dlm.AcquireDistributedLockAsync("DLM:AcquireDistributedLock").Result;

            Assert.True(distibutedLock != null && !distibutedLock.IsExpired && !distibutedLock.IsReleased);
        }

        [Fact(DisplayName = "DLM:CreateAndConnect")]
        public void CreateAndConnectAsyncTest()
        {
            var dlm = ScarletLock.Instance
                .BuildDistributedLockManager<Guid>(TimeSpan.FromSeconds(10),
                new ServerDetails { EndPoints = new EndPoint[] { new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6379) } }).Result;

            Assert.True(dlm != null);
        }

        [Fact(DisplayName = "DLM:ToString")]
        public void ToStringTest()
        {
            var dlm = ScarletLock.Instance
                .BuildDistributedLockManager<Guid>(TimeSpan.FromSeconds(10),
                new ServerDetails { EndPoints = new EndPoint[] { new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6379) } }).Result;

            Assert.True(!string.IsNullOrWhiteSpace(dlm.ToString()));
        }
    }
}