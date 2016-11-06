using System;
using CacheManager.Core;
using Microsoft.Extensions.Logging;

namespace Test
{
    public class Tests
    {
        public static void TestEachMethod(ICacheManager<string> cache)
        {
            cache.Clear();

            cache.Add("key", "value", "region");
            cache.AddOrUpdate("key", "region", "value", _ => "update value", 22);

            string testVal = null;
            if (!cache.TryGetOrAdd("key", "region", (k, r) => "really?", out testVal))
            {
                throw new Exception();
            }
            if(testVal != "update value")
            {
                throw new Exception();
            }

            cache.Expire("key", "region", TimeSpan.FromDays(1));
            var val = cache.Get("key", "region");
            var item = cache.GetCacheItem("key", "region");
            cache.Put("key", "put value", "region");
            cache.RemoveExpiration("key", "region");

            string update2;
            cache.TryUpdate("key", "region", _ => "update 2 value", out update2);

            var update3 = cache.Update("key", "region", _ => "update 3 value");

            cache.Remove("key", "region");

            cache.Clear();
            cache.ClearRegion("region");
        }
    }
}
