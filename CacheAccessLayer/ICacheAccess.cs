using DatabaseAccessLayer.Models;
using System;
using System.Threading.Tasks;

namespace CacheAccessLayer
{
    public interface ICacheAccess
    {
        Task<bool> Set<T>(T model, string key, TimeSpan expires) where T : EntityModel;
        Task<bool> Set<T>(T model, TimeSpan expires) where T : EntityModel;

        Task<(bool Ok, T Value)> Get<T>(string key) where T : EntityModel;
        Task<(bool Ok, T Value)> Get<T>(Guid id) where T : EntityModel;

        Task<bool> Remove<T>(string key) where T : EntityModel;
        Task<bool> Remove<T>(Guid id) where T : EntityModel;

        Task<bool> Refresh<T>(string key, TimeSpan expires) where T : EntityModel;
        Task<bool> Refresh<T>(Guid id, TimeSpan expires) where T : EntityModel;
    }
}
