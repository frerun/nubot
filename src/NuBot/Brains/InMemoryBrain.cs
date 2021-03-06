﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace NuBot.Brains
{
    public sealed class InMemoryBrain : IBrain
    {
        private static readonly ConcurrentDictionary<string, string> Store;
        private static readonly JavaScriptSerializer Serializer;

        static InMemoryBrain()
        {
            Store = new ConcurrentDictionary<string, string>();
            Serializer = new JavaScriptSerializer();
        }

        public Task<T> GetAsync<T>(string key)
        {
            string val;

            if (!Store.TryGetValue(key, out val))
            {
                throw new KeyNotFoundException(key);
            }

            return Task.FromResult(Serializer.Deserialize<T>(val));
        }

        public Task<T> GetAsync<T>(string key, T defaultValue)
        {
            string val;

            if (!Store.TryGetValue(key, out val))
            {
                return Task.FromResult(defaultValue);
            }

            return Task.FromResult(Serializer.Deserialize<T>(val));
        }

        public Task SetAsync<T>(string key, T value)
        {
            var json = Serializer.Serialize(value);
            Store.AddOrUpdate(key, json, (s, t) => t);
            return Task.FromResult(0);
        }
    }
}
