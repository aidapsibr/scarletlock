using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScarletLock.Redis;

namespace ScarletLock
{
    public static class ScarletLockExtensions
    {
        public static void UseRedis(this ScarletLock scarletLock)
        {
            ScarletLock.RegisterConnectionFactory(RedisConnectionFactory.BuildConnectionFactory);
        }
    }
}
