#region

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using Iam.Common.Contracts;
using JetBrains.Annotations;

#endregion

namespace Iam.Web.Services
{
    [UsedImplicitly]
    public class HttpCache : ICache
    {
        private const string CacheKey = "HttpCacheService";
        private readonly Cache _cache;

        public HttpCache()
        {
            _cache = HttpContext.Current.Cache;
        }

        public object Get(string key)
        {
            return _cache.Get($"{CacheKey}-{key}");
        }

        public void Put(string key, object value)
        {
            _cache.Insert($"{CacheKey}-{key}", value);
        }

        public void Put(string key, object value, TimeSpan timeout)
        {
            _cache.Insert($"{CacheKey}-{key}", value, null, DateTime.Now.Add(timeout), TimeSpan.Zero);
        }

        public void Remove(string key)
        {
            _cache.Remove($"{CacheKey}-{key}");
        }

        public void Clear()
        {
            var cacheItems = _cache.GetEnumerator();
            var keys = new List<string>();

            while (cacheItems.MoveNext())
            {
                keys.Add((string) cacheItems.Key);
            }

            foreach (var key in keys)
            {
                if (key.StartsWith(CacheKey))
                    _cache.Remove(key);
            }
        }
    }
}