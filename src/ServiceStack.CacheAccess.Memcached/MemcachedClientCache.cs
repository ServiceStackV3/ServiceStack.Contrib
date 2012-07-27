using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using ServiceStack.Common.Extensions;
using ILog = ServiceStack.Logging.ILog;
using LogManager = ServiceStack.Logging.LogManager;

namespace ServiceStack.CacheAccess.Memcached
{
	/// <summary>
	/// A memcached implementation of the ServiceStack ICacheClient interface.
	/// Good practice not to have dependencies on implementations in your business logic.
	/// 
	/// Basically delegates all calls to Enyim.Caching.MemcachedClient with added diagnostics and logging.
	/// </summary>
	public class MemcachedClientCache 
		: AdapterBase, ICacheClient, IMemcachedClient
	{
		protected override ILog Log { get { return LogManager.GetLogger(GetType()); } }

		private MemcachedClient _client;

		public MemcachedClientCache(IEnumerable<string> hosts)
		{
			const int defaultPort = 11211;
			const int ipAddressIndex = 0;
			const int portIndex = 1;

			_client = new MemcachedClient();

			var ipEndpoints = new List<IPEndPoint>();
			foreach (var host in hosts)
			{
				var hostParts = host.Split(':');
				if (hostParts.Length == 0)
					throw new ArgumentException("'{0}' is not a valid host IP Address: e.g. '127.0.0.0[:11211]'");

				var port = (hostParts.Length == 1) ? defaultPort : int.Parse(hostParts[portIndex]);

				var hostAddresses = Dns.GetHostAddresses(hostParts[ipAddressIndex]);
				foreach (var ipAddress in hostAddresses)
				{
					var endpoint = new IPEndPoint(ipAddress, port);
					ipEndpoints.Add(endpoint);
				}
			}
			LoadClient(ipEndpoints);
		}

		public MemcachedClientCache(IEnumerable<IPEndPoint> ipEndpoints)
		{
			LoadClient(ipEndpoints);
		}

		private void LoadClient(IEnumerable<IPEndPoint> ipEndpoints)
		{
			var config = new MemcachedClientConfiguration();
			foreach (var ipEndpoint in ipEndpoints)
			{
				config.Servers.Add(ipEndpoint);
			}

			config.SocketPool.MinPoolSize = 10;
			config.SocketPool.MaxPoolSize = 100;
			config.SocketPool.ConnectionTimeout = new TimeSpan(0, 0, 10);
			config.SocketPool.DeadTimeout = new TimeSpan(0, 2, 0);

			_client = new MemcachedClient(config);
		}

		public MemcachedClientCache(MemcachedClient client)
		{
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}
			_client = client;
		}

		public void Dispose()
		{
            /* 
             * DO NOTHING!! 
             * 
             * Calling _client.Dispose() breaks any call to a service that uses ICachClient 
             * after a call to ServiceStack.ServiceInterface.ServiceExtension.GetSession.
             * 
             * Enyim.Caching.MemcachedClient defines a destructor that handles all necessary cleanup (disposing is done there, we don't need to worry).
             */
        }

		public bool Remove(string key)
		{
			return Execute(() => _client.Remove(key));
		}

		public object Get(string key)
		{
			return Execute(() => ((MemcachedValueWrapper)_client.Get(key)).Value);
		}

		public object Get(string key, out ulong ucas)
		{
			IDictionary<string, ulong> casValues;
			var results = GetAll(new[] { key }, out casValues);

			object result;
			if (results.TryGetValue(key, out result))
			{
				ucas = casValues[key];
				return result;
			}

			ucas = default(ulong);
			return null;
		}

		public T Get<T>(string key)
		{
            return Execute(() =>
                               {
                                   var result = _client.Get<MemcachedValueWrapper>(key);
                                   if (result != null)
                                       return (T)result.Value;
                                    return default(T);
                               });
		}

		public T Get<T>(string key, out ulong ucas)
		{
			IDictionary<string, ulong> casValues;
			var results = GetAll(new[] { key }, out casValues);

			object result;
			if (results.TryGetValue(key, out result))
			{
				ucas = casValues[key];
				return (T)result;
			}

			ucas = default(ulong);
			return default(T);
		}

		public long Increment(string key, uint amount)
		{
			return Execute(() => (long)_client.Increment(key, 0, amount));
		}

		public long Decrement(string key, uint amount)
		{
			return Execute(() => (long)_client.Decrement(key, 0, amount));
		}

		public bool Add<T>(string key, T value)
		{
			return Execute(() => _client.Store(StoreMode.Add, key, new MemcachedValueWrapper(value)));
		}

		public bool Set<T>(string key, T value)
		{
            return Execute(() => _client.Store(StoreMode.Set, key, new MemcachedValueWrapper(value)));
		}

		public bool Replace<T>(string key, T value)
		{
            return Execute(() => _client.Store(StoreMode.Replace, key, new MemcachedValueWrapper(value)));
		}

		public bool Add<T>(string key, T value, DateTime expiresAt)
		{
            return Execute(() => _client.Store(StoreMode.Add, key, new MemcachedValueWrapper(value), expiresAt));
		}

		public bool Set<T>(string key, T value, DateTime expiresAt)
		{
            return Execute(() => _client.Store(StoreMode.Set, key, new MemcachedValueWrapper(value), expiresAt));
		}

		public bool Replace<T>(string key, T value, DateTime expiresAt)
		{
            return Execute(() => _client.Store(StoreMode.Replace, key, new MemcachedValueWrapper(value), expiresAt));
		}

		public bool Add<T>(string key, T value, TimeSpan expiresIn)
		{
            return Execute(() => _client.Store(StoreMode.Add, key, new MemcachedValueWrapper(value), expiresIn));
		}

		public bool Set<T>(string key, T value, TimeSpan expiresIn)
		{
            return Execute(() => _client.Store(StoreMode.Set, key, new MemcachedValueWrapper(value), expiresIn));
		}

		public bool Replace<T>(string key, T value, TimeSpan expiresIn)
		{
            return Execute(() => _client.Store(StoreMode.Replace, key, new MemcachedValueWrapper(value), expiresIn));
		}

		public bool Add(string key, object value)
		{
            return Execute(() => _client.Store(StoreMode.Add, key, new MemcachedValueWrapper(value)));
		}

		public bool Set(string key, object value)
		{
            return Execute(() => _client.Store(StoreMode.Set, key, new MemcachedValueWrapper(value)));
		}

		public bool Replace(string key, object value)
		{
            return Execute(() => _client.Store(StoreMode.Replace, key, new MemcachedValueWrapper(value)));
		}

		public bool Add(string key, object value, DateTime expiresAt)
		{
            return Execute(() => _client.Store(StoreMode.Add, key, new MemcachedValueWrapper(value), expiresAt));
		}

		public bool Set(string key, object value, DateTime expiresAt)
		{
            return Execute(() => _client.Store(StoreMode.Set, key, new MemcachedValueWrapper(value), expiresAt));
		}

		public bool Replace(string key, object value, DateTime expiresAt)
		{
            return Execute(() => _client.Store(StoreMode.Replace, key, new MemcachedValueWrapper(value), expiresAt));
		}

		public bool CheckAndSet(string key, object value, ulong cas)
		{
            return Execute(() => _client.Cas(StoreMode.Replace, key, new MemcachedValueWrapper(value), cas).Result);
		}

		public bool CheckAndSet(string key, object value, ulong cas, DateTime expiresAt)
		{
            return Execute(() => _client.Cas(StoreMode.Replace, key, new MemcachedValueWrapper(value), expiresAt, cas).Result);
		}

		public void FlushAll()
		{
			Execute(() => _client.FlushAll());
		}

		public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
		{
			var results = new Dictionary<string, T>();
			foreach (var key in keys)
			{
				var result = Get<T>(key);
				results[key] = result;
			}

			return results;
		}

		public void SetAll<T>(IDictionary<string, T> values)
		{
			foreach (var entry in values)
			{
				Set(entry.Key, new MemcachedValueWrapper(entry.Value));
			}
		}

		public IDictionary<string, object> GetAll(IEnumerable<string> keys)
		{
            var results = new Dictionary<string, object>();
            foreach (var key in keys)
            {
                var result = Get(key);
                results[key] = result;
            }

            return results;
		}

		public IDictionary<string, object> GetAll(IEnumerable<string> keys, out IDictionary<string, ulong> casValues)
		{
		    var retVal = new Dictionary<string, object>();
		    casValues = new Dictionary<string, ulong>();
		    foreach (var casResult in _client.GetWithCas(keys))
		    {
		        retVal.Add(casResult.Key, ((MemcachedValueWrapper)casResult.Value.Result).Value);
		        casValues.Add(casResult.Key, casResult.Value.Cas);
		    }
		    return retVal;
		}

		public void RemoveAll(IEnumerable<string> keys)
		{
			foreach (var key in keys)
			{
				try
				{
					Remove(key);
				}
				catch (Exception ex)
				{
					Log.Error(string.Format("Error trying to remove {0} from memcached", key), ex);
				}
			}
		}
	}
}