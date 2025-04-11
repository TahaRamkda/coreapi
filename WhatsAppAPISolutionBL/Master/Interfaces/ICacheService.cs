using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ICacheService
    {
        T Get<T>(string key, Func<T> acquire);
        T Get<T>(string key, Func<T> acquire, int cacheTime);
        Task<T> GetAsync<T>(string key, Func<Task<T>> acquire);
        Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int cacheTime);
        Task<T> SetAsync<T>(string key, Func<Task<T>> acquire);
        Task<T> SetAsync<T>(string key, Func<Task<T>> acquire, int cacheTime);
        Task RemoveAsync(string key);
        Task RemoveByPrefix(string prefix);
        Task Clear(bool publisher = true);
    }
}
