using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using System.Net.Http;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Setting;
using Microsoft.AspNetCore.Http;
using WhatsAppAPISolutionDL.Dto.Common;
using Newtonsoft.Json;
using System.Text;


namespace WhatsAppAPISolutionBL.Master.Services
{
    public class CacheService : ICacheService
    {
        #region Fields

        private readonly IMemoryCache _cache;
        private readonly CacheSettings _cacheSettings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CacheService> _logger;

        private static CancellationTokenSource _resetCacheToken = new();

        protected readonly ConcurrentDictionary<string, SemaphoreSlim> CacheEntries = new();

        #endregion

        #region Ctor

        public CacheService(IHttpClientFactory httpClientFactory, IMemoryCache cache, IOptions<CacheSettings> cacheSettings, ILogger<CacheService> logger, HttpClient httpClient)
        {
            _cache = cache;
            _cacheSettings = cacheSettings.Value;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _logger = logger;
        }

        #endregion

        #region Methods

        public virtual T Get<T>(string key, Func<T> acquire)
        {
            return Get(key, acquire, _cacheSettings.DefaultExpirationInMinutes);
        }

        public virtual T Get<T>(string key, Func<T> acquire, int cacheTime)
        {
            if (_cache.TryGetValue(key, out T cacheEntry)) return cacheEntry;
            var semaphore = CacheEntries.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            semaphore.Wait();
            try
            {
                if (!_cache.TryGetValue(key, out cacheEntry))
                {
                    cacheEntry = acquire();
                    _cache.Set(key, cacheEntry, GetMemoryCacheEntryOptions(cacheTime));
                }
            }
            finally
            {
                semaphore.Release();
            }

            return cacheEntry;
        }

        public virtual Task<T> GetAsync<T>(string key, Func<Task<T>> acquire)
        {
            return GetAsync(key, acquire, _cacheSettings.DefaultExpirationInMinutes);
        }

        public virtual async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int cacheTime)
        {
            //If caching is not enabled, return the object directly
            if (!_cacheSettings.CachingEnabled)
                return await acquire();

            if (_cache.TryGetValue(key, out T cacheEntry))
                return cacheEntry;

            var semaphore = CacheEntries.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();
            try
            {
                if (!_cache.TryGetValue(key, out cacheEntry))
                {
                    cacheEntry = await acquire();
                    _cache.Set(key, cacheEntry, GetMemoryCacheEntryOptions(cacheTime));
                }
            }
            finally
            {
                semaphore.Release();
            }

            return cacheEntry;
        }

        public virtual Task<T> SetAsync<T>(string key, Func<Task<T>> acquire)
        {
            return SetAsync(key, acquire, _cacheSettings.DefaultExpirationInMinutes);
        }

        public virtual async Task<T> SetAsync<T>(string key, Func<Task<T>> acquire, int cacheTime)
        {
            var semaphore = CacheEntries.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();
            try
            {
                var cacheEntry = await acquire();
                _cache.Set(key, cacheEntry, GetMemoryCacheEntryOptions(cacheTime));
                return cacheEntry;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public virtual Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        public virtual Task RemoveByPrefix(string prefix)
        {
            var entriesToRemove = CacheEntries.Where(x => x.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            foreach (var cacheEntries in entriesToRemove) _cache.Remove(cacheEntries.Key);

            return Task.CompletedTask;
        }

        public virtual Task Clear(bool publisher = true)
        {
            //clear keys
            foreach (var cacheEntry in CacheEntries.Keys.ToList())
                _cache.Remove(cacheEntry);

            //cancel
            _resetCacheToken.Cancel();
            //dispose
            _resetCacheToken.Dispose();

            _resetCacheToken = new CancellationTokenSource();

            return Task.CompletedTask;
        }

        #endregion

        #region Utilities

        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions(int cacheTime)
        {
            var options = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTime) }
                .AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token))
                .RegisterPostEvictionCallback(PostEvictionCallback);

            return options;
        }

        private void PostEvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            if (reason != EvictionReason.Replaced)
                CacheEntries.TryRemove(key.ToString(), out var _);
        }

        #endregion

        public async Task<ApiResult> ClearBridgeCacheAsync()
        {
            _logger.LogInformation("Calling Bridge ClearBridgeCacheAsync.");
            var bridgeEndpoint = "/api/Cache/Clear";
            var response =  await _httpClient.PostAsync(bridgeEndpoint, null);
            if(!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to clear bridge cache.Status Code: {StatusCode}", response.StatusCode);
                return new ApiResult
                {
                    Success = false,
                    Message = "Failed to clear bridge cache",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            _logger.LogInformation("Bridge cache cleared successfully.");
            return new ApiResult
            {
                Success = true,
                Message = "Bridge cache cleared successfully.",
                StatusCode = StatusCodes.Status200OK
            };
        }
        public async Task<ApiResult> ClearBridgeCachebyPrefixAsync(string prefix)
        {
            _logger.LogInformation("Calling bridge api with the prefix={prefix}", prefix);
            // Prepare request payload for the endpoint 
            var requestDto = new { Prefix = prefix }; //creating a dynamix dto for the 
            var json = JsonConvert.SerializeObject(requestDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // Call the bridge endpoint
            var bridgeEndpoint = "api/Cache/ClearByPrefix";
            var response = await _httpClient.PostAsync(bridgeEndpoint, content);

            // Handle failure
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Bridge API ClearByPrefix failed. StatusCode={StatusCode}", response.StatusCode);
                return new ApiResult
                {
                    Success = false,
                    Message = "Failed to clear bridge cache by prefix.",
                    StatusCode = (int)response.StatusCode
                };
            }

            var payload = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Bridge cache cleared by prefix successfully.");

            return new ApiResult
            {
                Success = true,
                Message = "Bridge cache cleared by prefix successfully.",
                StatusCode = StatusCodes.Status200OK
            };

        }
    }
}
