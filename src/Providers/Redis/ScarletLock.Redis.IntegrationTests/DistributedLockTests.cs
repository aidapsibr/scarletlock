using System;
using System.Net;
using Xunit;

namespace ScarletLock.Redis.IntegrationTests
{
    public class DistributedLockTests
    {
        public DistributedLockTests()
        {
            ScarletLock.Instance
                .UseRedis();

        }

        [Fact(DisplayName = "DistributedLockRelease")]
        public void ReleaseTest()
        {
            var dlm = ScarletLock.Instance
                .BuildDistributedLockManager<Guid>(TimeSpan.FromSeconds(10),
                new ServerDetails { EndPoints = new EndPoint[] { new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6379) } }).Result;

            var distibutedLock = dlm.AcquireDistributedLockAsync("DistributedLockRelease").Result;

            Assert.True(!distibutedLock.IsReleased);

            distibutedLock.Release();

            Assert.True(distibutedLock.IsReleased);
        }

        [Fact(DisplayName = "DistributedLockReleaseUsing")]
        public void ReleaseUsingTest()
        {
            var dlm = ScarletLock.Instance
                .BuildDistributedLockManager<Guid>(TimeSpan.FromSeconds(10),
                new ServerDetails { EndPoints = new EndPoint[] { new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6379) } }).Result;

            IDistributedLock<Guid> leak;
            using (var distibutedLock = dlm.AcquireDistributedLockAsync("DistributedLockReleaseUsing").Result)
            {
                Assert.True(!distibutedLock.IsReleased);
                leak = distibutedLock;
            }
            Assert.True(leak.IsReleased);

        }
    }
}