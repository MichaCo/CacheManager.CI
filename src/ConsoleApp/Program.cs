using System;
using System.Threading;
using CacheManager.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Test;

namespace ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = CacheManager.Core.ConfigurationBuilder.BuildConfiguration(
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
                    s.WithProtoBufSerializer();
                    s.WithJsonSerializer();
                    //s.WithRedisBackplane("redis");
                    //s.WithRedisConfiguration("redis",
                    //    cfg =>
                    //    cfg.WithEndpoint("127.0.0.1", 6379)
                    //        .EnableKeyspaceEvents()
                    //        .WithDatabase(0)
                    //        .WithAllowAdmin());
                    s.WithDictionaryHandle("dic1");
                    s.WithMicrosoftMemoryCacheHandle("ms1");
                    s.WithSystemRuntimeCacheHandle("runtime1")
                        .EnablePerformanceCounters()
                        .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(2));

                    s.WithDictionaryHandle("dic2")
                        .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromSeconds(5));

                    //s.WithRedisCacheHandle("redis", true)
                    //    .EnablePerformanceCounters()
                    //    .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromSeconds(5));
                });

            var cache = CacheFactory.FromConfiguration<Poco>(config);
            cache.Clear();
            
            Tests.TestEachMethod(CacheFactory.FromConfiguration<string>(config));
            Tests.TestPoco(CacheFactory.FromConfiguration<Poco>(config));

            // json test
            var logConfig = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddJsonFile("cache.json").Build();

            var cacheConfig = logConfig.GetCacheConfiguration()
                .Builder
                .WithMicrosoftLogging(f =>
                {
                    f.AddDebug(LogLevel.Trace);
                    f.AddConsole(LogLevel.Trace);
                })
                .Build();

            var fromJsonCache = new BaseCacheManager<string>(cacheConfig);

            Tests.TestEachMethod(fromJsonCache);
        }
    }
}