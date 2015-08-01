using System;
using System.Net;
using System.Threading;

namespace ScarletLock.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            new DependencyModule().Register();

            var dlm = DistributedLockManager<Guid>.CreateAndConnectAsync(TimeSpan.FromSeconds(10),
                new ServerDetails { EndPoints = new EndPoint[] { new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6379) } }).Result;

            while (true)
            {

                var testLock = dlm.AcquireDistributedLockAsync("test").Result;
                
                    Console.WriteLine(testLock == null ? "Lock held :[" : "I has the lock");
                
                Thread.Sleep(1000);
            }
        }
    }
}
