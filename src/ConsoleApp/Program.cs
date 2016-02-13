using System;
using CacheManager.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Test;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
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
                            f.MinimumLevel = LogLevel.Debug;
                            f.AddDebug(LogLevel.Debug);
                            f.AddProvider(new MyConsoleLoggerProviderBecauseRC1DoesntWork());
                        });
                    s.WithJsonSerializer();
                    //s.WithRedisBackPlate("redisConfigKey");
                    //s.WithRedisConfiguration("redisConfigKey",
                    //    cfg =>
                    //    cfg.WithEndpoint("127.0.0.1", 6379)
                    //    .WithDatabase(0)
                    //    .WithAllowAdmin());

                    s.WithSystemRuntimeDefaultCacheHandle()
                        .EnablePerformanceCounters()
                        .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10));

                    //s.WithRedisCacheHandle("redisConfigKey", true)
                    //    .EnablePerformanceCounters()
                    //    .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(2));
                });

            cache.Clear();

            Tests.TestEachMethod(cache);

            // json test
            var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddJsonFile("cache.json").Build();

            var cacheConfig = config.GetCacheConfiguration()
                .WithMicrosoftLogging(f =>
                {
                    f.MinimumLevel = LogLevel.Debug;
                    f.AddDebug(LogLevel.Debug);
                    f.AddProvider(new MyConsoleLoggerProviderBecauseRC1DoesntWork());
                });

            var fromJsonCache = new BaseCacheManager<string>(cacheConfig);

            Tests.TestEachMethod(fromJsonCache);
        }
    }
}
