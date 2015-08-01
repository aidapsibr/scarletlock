using System;
using System.Net;
using ScarletLock.Tests.Mocks;
using ScarletLock.TinyIoC;
using Xunit;

namespace ScarletLock.Tests
{
    public class DistributedLockManagerTests
    {
        public DistributedLockManagerTests()
        {
            var container = TinyIoCContainer.Current;

            container
                .Register((kernel, overloads) =>
                    DistributedLockFactory<Guid>.Create(Guid.NewGuid));

            container
                .Register<IConnectionFactory>((kernel, oerloads) =>
                    new MockConnectionFactory());

        }

        [Fact(DisplayName = "DLM:ReleaseDistributedLock")]
        public void ReleaseDistributedLockAsyncTest()
        {
            var dlm = DistributedLockManager<Guid>.CreateAndConnectAsync(TimeSpan.FromSeconds(10),
                new ServerDetails { EndPoints = new EndPoint[] { new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80) } }).Result;

            var distibutedLock = dlm.AcquireDistributedLockAsync("DLM:ReleaseDistributedLock").Result;

            dlm.ReleaseDistributedLockAsync(distibutedLock).Wait();
        }

        [Fact(DisplayName = "DLM:AcquireDistributedLock")]
        public void AcquireDistributedLockAsyncTest()
        {
            var dlm = DistributedLockManager<Guid>.CreateAndConnectAsync(TimeSpan.FromSeconds(10),
                new ServerDetails { EndPoints = new EndPoint[] { new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80) } }).Result;

            var distibutedLock = dlm.AcquireDistributedLockAsync("DLM:AcquireDistributedLock").Result;

            Assert.True(distibutedLock != null && !distibutedLock.IsExpired && !distibutedLock.IsReleased);
        }

        [Fact(DisplayName = "DLM:CreateAndConnect")]
        public void CreateAndConnectAsyncTest()
        {
            var dlm = DistributedLockManager<Guid>.CreateAndConnectAsync(TimeSpan.FromSeconds(10),
                new ServerDetails { EndPoints = new EndPoint[] { new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80) } }).Result;

            Assert.True(dlm != null);
        }

        [Fact(DisplayName = "DLM:ToString")]
        public void ToStringTest()
        {
            var dlm = DistributedLockManager<Guid>.CreateAndConnectAsync(TimeSpan.FromSeconds(10),
                new ServerDetails { EndPoints = new EndPoint[] { new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80) } }).Result;

            Assert.True(!string.IsNullOrWhiteSpace(dlm.ToString()));
        }
    }
}