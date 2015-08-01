using System;
using ScarletLock.TinyIoC;

namespace ScarletLock
{
    public class DependencyModule
    {
        public void Register()
        {
            var container = TinyIoCContainer.Current;

            container
                .Register((kernel, overloads) =>
                    DistributedLockFactory<Guid>.Create(Guid.NewGuid));

            container
                .Register((kernel, oerloads) =>
                    RedisConnectionFactory.BuildConnectionFactory());
        }
    }
}