using System;
using CacheManager.Core;
using Microsoft.Extensions.Logging;
using Test;

namespace DotNetCoreApp
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
                    s.WithJsonSerializer();
                    s.WithUpdateMode(CacheUpdateMode.Up);
                    s.WithJsonSerializer();
                    s.WithMicrosoftLogging(
                        f =>
                        {
                            f.AddDebug(LogLevel.Trace);
                            f.AddConsole(LogLevel.Trace);
                        });

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

                });

            cache.Clear();
            Tests.TestEachMethod(cache);
        }
    }
}
