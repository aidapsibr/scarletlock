using System;
using System.Net;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ScarletLock.Redis
{
    public class RedisConnection
        : IConnection
    {
        /// <summary>
        /// String containing the Lua unlock script.
        /// </summary>
        protected const string UnlockScript = @"
            if redis.call(""get"",KEYS[1]) == ARGV[1] then
                return redis.call(""del"",KEYS[1])
            else
                return 0
            end";

        protected ConnectionMultiplexer RedisConnectionMultiplexer { get; }

        public RedisConnection(ConnectionMultiplexer connection)
        {
            RedisConnectionMultiplexer = connection;
        }

        public EndPoint[] Endpoints => RedisConnectionMultiplexer.GetEndPoints();

        public async Task<bool> SetStringWhenNotSetAsync(string resource, string identity, TimeSpan ttl)
        {
            return await RedisConnectionMultiplexer.GetDatabase()
                .StringSetAsync(resource, identity, ttl, When.NotExists);
        }

        public async Task DeleteIfMatchedAsync(string resource, string identity)
        {
            await RedisConnectionMultiplexer.GetDatabase()
                .ScriptEvaluateAsync(UnlockScript, new RedisKey[] { resource },
                    new RedisValue[] { identity });
        }

        public void Close()
        {
            RedisConnectionMultiplexer.Close();
        }

        public async static Task<IConnection> ConnectAsync(EndPoint[] endpoints)
        {
            var configuration = new ConfigurationOptions { ClientName = "ScarletLock" };

            foreach (var endpoint in endpoints)
                configuration.EndPoints.Add(endpoint);

            return new RedisConnection(await ConnectionMultiplexer.ConnectAsync(configuration));
        }
    }
}
