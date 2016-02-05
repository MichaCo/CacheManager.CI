using System;
using CacheManager.Core;
using Microsoft.Extensions.Logging;

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
                    s.WithAspNetLogging(
                        f => 
                        f.AddDebug(LogLevel.Verbose)
                        .AddProvider(new MyConsoleLoggerProviderBecauseRC1DoesntWork()));
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

            TestEachMethod(cache);
        }

        public static void TestEachMethod(ICacheManager<string> cache)
        {
            cache.Clear();

            cache.Add("key", "value", "region");
            cache.AddOrUpdate("key", "region", "value", _ => "update value", 22);

            cache.Expire("key", "region", TimeSpan.FromDays(1));
            var val = cache.Get("key", "region");
            var item = cache.GetCacheItem("key", "region");
            cache.Put("key", "region", "put value");
            cache.RemoveExpiration("key", "region");

            string update2;
            cache.TryUpdate("key", "region", _ => "update 2 value", out update2);

            var update3 = cache.Update("key", "region", _ => "update 3 value");

            cache.Remove("key", "region");

            cache.Clear();
            cache.ClearRegion("region");
        }
    }

    public class MyConsoleLoggerProviderBecauseRC1DoesntWork : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new MyConsoleLoggerBecauseRC1DoesntWork(categoryName);
        }

        public void Dispose()
        {
        }
    }

    public class MyConsoleLoggerBecauseRC1DoesntWork : ILogger
    {
        public MyConsoleLoggerBecauseRC1DoesntWork(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        public IDisposable BeginScopeImpl(object state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            Console.WriteLine("{0}: {1}: {2}", this.Name, logLevel, formatter(state, exception));
        }
    }
}
