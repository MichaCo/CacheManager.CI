using CacheManager.Core;
using Microsoft.Extensions.Logging;
using System;

namespace NetCore2App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var cache = CacheFactory.Build<string>(
            s =>
            {
                s.WithMaxRetries(50);
                s.WithRetryTimeout(100);
                s.WithUpdateMode(CacheUpdateMode.Up);
                s.WithMicrosoftLogging(
                    f =>
                    {
                        f.AddDebug(LogLevel.Trace);
                        f.AddConsole(LogLevel.Trace);
                    });

                s.WithJsonSerializer();
                s.WithBondSimpleJsonSerializer();
                s.WithProtoBufSerializer();
                s.WithDataContractBinarySerializer().WithDataContractGzJsonSerializer();
                
                s.WithDictionaryHandle();

                s.WithSystemRuntimeCacheHandle()
                    .EnablePerformanceCounters()
                    .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10));

                s.WithCouchbaseConfiguration("couch", new Couchbase.Configuration.Client.ClientConfiguration());
                s.WithCouchbaseCacheHandle("couch", isBackplaneSource: false);

                s.WithRedisConfiguration("redisConfigKey",
                    cfg =>
                    cfg.WithEndpoint("127.0.0.1", 6379)
                    .WithDatabase(0)
                    .WithAllowAdmin());
                s.WithRedisCacheHandle("redisConfigKey", true)
                    .EnablePerformanceCounters()
                    .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(2));

            });
        }
    }
}