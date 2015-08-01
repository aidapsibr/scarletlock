using System;
using System.Net;
using Xunit;

namespace ScarletLock.Tests
{
    public class DistributedLockTests
    {
        [Fact(DisplayName = "DistributedLockRelease")]
        public void ReleaseTest()
        {
            var dlm = DistributedLockManager<Guid>.CreateAndConnectAsync(TimeSpan.FromSeconds(10),
                new ServerDetails { EndPoints = new EndPoint[] { new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80) } }).Result;

            var distibutedLock = dlm.AcquireDistributedLockAsync("DistributedLockRelease").Result;

            Assert.True(!distibutedLock.IsReleased);

            distibutedLock.Release();

            Assert.True(distibutedLock.IsReleased);
        }

        [Fact(DisplayName = "DistributedLockReleaseUsing")]
        public void ReleaseUsingTest()
        {
            var dlm = DistributedLockManager<Guid>.CreateAndConnectAsync(TimeSpan.FromSeconds(10),
                new ServerDetails { EndPoints = new EndPoint[] { new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80) } }).Result;
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