using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    public class Cache
    {
        private Dictionary<string, object> cache;
        static readonly object cacheLock = new object();

        public Cache()
        {
            Initialize();
        }
        public void Initialize()
        {
            lock (cacheLock)
            {
                cache = new Dictionary<string, object>();
            }

        }
        public void Add(string key, object value)
        {
            lock (cacheLock)
            {
                if (cache.ContainsKey(key))
                {
                     throw new InvalidOperationException(key + " Already Exists");
                }
                cache[key] = value;
            }
        }

        public void Clear()
        {
            lock (cacheLock)
            {
                cache.Clear();
            }
        }

 

        public object Get(string key)
        {
            lock (cacheLock)
            {
                object value;
                if (!cache.ContainsKey(key))
                {
                    throw new InvalidOperationException(key + " Dont Exists");

                }
                cache.TryGetValue(key, out value);
                return value;
            }
        }



        public void Remove(string key)
        {
            lock (cacheLock)
            {
                if (!cache.ContainsKey(key))
                {
                    throw new InvalidOperationException(key + " Dont Exists");

                }
                cache.Remove(key);
            }
        }
    }
}
