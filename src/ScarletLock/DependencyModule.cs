using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScarletLock
{
    public class DependencyModule
    {
        public void Register()
        {
            var container = TinyIoC.TinyIoCContainer.Current;

            container
                .Register((kernel, overloads) =>
                    DistributedLockFactory<Guid>.Create(() => Guid.NewGuid(), TimeSpan.FromSeconds(10)))
                .AsSingleton();
        }
    }
}