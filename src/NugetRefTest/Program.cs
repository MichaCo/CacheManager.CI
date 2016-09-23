using System;
using CacheManager.Core;
using Microsoft.Extensions.Logging;

namespace NugetRefTest
{
    public class Program
    {
        public static void Main(string[] args)
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
                    s.WithDictionaryHandle();
#if !NETCORE
                    s.WithSystemRuntimeCacheHandle()
                        .EnablePerformanceCounters()
                        .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10));

                    s.WithCouchbaseConfiguration("couch", new Couchbase.Configuration.Client.ClientConfiguration());
                    s.WithCouchbaseCacheHandle("couch");

                    s.WithMemcachedCacheHandle("memcached");

                    s.WithRedisBackplane("redisConfigKey");
                    s.WithRedisConfiguration("redisConfigKey",
                        cfg =>
                        cfg.WithEndpoint("127.0.0.1", 6379)
                        .WithDatabase(0)
                        .WithAllowAdmin());
                    s.WithRedisCacheHandle("redisConfigKey", true)
                        .EnablePerformanceCounters()
                        .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(2));
#endif
                });
        }
    }
}