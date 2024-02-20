using eXtensionSharp;
using ServiceStack.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Jina.Cache
{
    public class RedisCache : IDisposable
    {
        private readonly RedisClient _client;

        /// <summary>
        /// ctor    
        /// </summary>
        /// <param name="endpoint"></param>
        /// <example>
        /// <code>
        /// var redisCache = new RedisCache(new RedisEndpoint(host:"", port:6, password:"test", db:1));
        /// </code>
        /// </example>
        public RedisCache(RedisEndpoint endpoint)
        {
            _client = new RedisClient(endpoint);
        }


        /// <summary>
        /// get cache data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// var v = redisCache.Get<string>("item");
        /// var v = redisCache.Get<string>("item:3");
        /// </code>
        /// </example>
        public T Get<T>(string searchKey)
        {
            var v = _client.Get<T>(searchKey);
            if (v.xIsNotEmpty()) return v;

            return default(T);
        }

        /// <summary>
        /// get cache datum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchKeys"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// var v = redisCache.Gets<string>("item*");
        /// </code>
        /// </example>
        public IList<T> Gets<T>(string searchKeys)
        {
            var keys = _client.SearchKeys(searchKeys);
            if(keys.Any())
            {
                List<T> list = _client.GetAll<T>(keys)
                    .Values.ToList();
                return list;
            }
            return default(IList<T>);
        }

        public bool Set<T>(string key, T value, DateTimeOffset expirationTime)
        {
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = _client.Set<T>(key, value, expiryTime);
            return isSet;
        }

        public bool Remove(string key)
        {
            bool _isKeyExist = _client.ContainsKey(key);
            if (_isKeyExist)
            {
                return _client.Remove(key);
            }
            return false;
        }

        /// <summary>
        /// set list data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setter"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// var setter = new RedisListSetter<string>();
        /// setter.MainKey = "Category";
        /// setter.RedisSetters = new List<RedisSetter<string>>() {
        ///     new RedisSetter<string>() {SubKey = "1", Value = "test1"},
        ///     new RedisSetter<string>() {SubKey = "2", Value = "test2"},
        /// }
        /// var seted = redisCache.Sets<string>(setter)
        /// </code>
        /// </example>
        public bool Sets<T>(RedisListSetter<T> setter)
        {
            foreach(var item in setter.RedisSetters) {
                _client.Set($"{setter.MainKey}:{item.SubKey}", item.Value);
            }
            return true;
        }
        
        public void Dispose()
        {
            if (_client.xIsNotEmpty()) _client.Dispose();
        }
    }

    public class RedisListSetter<T>
    {
        public string MainKey { get; set; }
        public IEnumerable<RedisSetter<T>> RedisSetters { get; set; }
    }

    public class RedisSetter<T>
    {
        public string SubKey { get; set; }
        public T Value { get; set; }
    }
}
