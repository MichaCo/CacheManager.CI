using System;
using CacheManager.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Test;

namespace DotNetCoreApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddLogging(c =>
            {
                c.AddConsole();
                c.AddDebug();
                c.SetMinimumLevel(LogLevel.Trace);
            });

            using var p = services.BuildServiceProvider();

            using var cache = CacheFactory.Build<string>(
                s =>
                {
                    s.WithMaxRetries(50);
                    s.WithRetryTimeout(100);
                    s.WithJsonSerializer();
                    s.WithUpdateMode(CacheUpdateMode.Up);
                    s.WithJsonSerializer();

                    s.WithDictionaryHandle();

                    //s.WithJsonSerializer();
                    //s.WithRedisBackplane("redisConfigKey");
                    //s.WithRedisConfiguration("redisConfigKey",
                    //    cfg =>
                    //    cfg.WithEndpoint("127.0.0.1", 6379)
                    //    .WithDatabase(0)
                    //    .WithAllowAdmin());
                    //s.WithRedisCacheHandle("redisConfigKey", true)
                    //    .EnablePerformanceCounters()
                    //    .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(2));

                },
                loggerFactory: p.GetRequiredService<ILoggerFactory>());

            cache.Clear();
            Tests.TestEachMethod(cache);
        }
    }
}
