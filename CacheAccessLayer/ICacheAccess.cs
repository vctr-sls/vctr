using DatabaseAccessLayer.Models;
using System;
using System.Threading.Tasks;

namespace CacheAccessLayer
{
    /// <summary>
    /// Data access provider for caching modules.
    /// </summary>
    public interface ICacheAccess
    {
        /// <summary>
        /// Sets an <see cref="EntityModel"/> to the cache by
        /// given key <see cref="string"/> for the specified
        /// <see cref="TimeSpan"/>.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="model">entity model instance</param>
        /// <param name="key">cache key string</param>
        /// <param name="expires">expiration time span</param>
        /// <returns></returns>
        Task<bool> Set<T>(T model, string key, TimeSpan expires) where T : EntityModel;

        /// <summary>
        /// Sets an <see cref="EntityModel"/> to the cache by
        /// the <see cref="Guid"/> contained in the given <see cref="EntityModel"/>
        /// instance for the specified <see cref="TimeSpan"/>.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="model">entity model instance</param>
        /// <param name="expires">expiration time span</param>
        /// <returns></returns>
        Task<bool> Set<T>(T model, TimeSpan expires) where T : EntityModel;

        /// <summary>
        /// Tries to retrieve the given <see cref="EntityModel"/> instance
        /// from cache by given <see cref="string"/> key.<br />
        /// Returns a tuple consisting of the access success status and
        /// the retrieved value.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="key">cache key</param>
        /// <returns></returns>
        Task<(bool Ok, T Value)> Get<T>(string key) where T : EntityModel;

        /// <summary>
        /// Tries to retrieve the given <see cref="EntityModel"/> instance
        /// from cache by given <see cref="Guid"/> key.<br />
        /// Returns a tuple consisting of the access success status and
        /// the retrieved value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<(bool Ok, T Value)> Get<T>(Guid id) where T : EntityModel;

        /// <summary>
        /// Removes a given <see cref="EntityModel"/> instance from the cache
        /// by given key <see cref="string"/>.<br />
        /// Returns true when the key was present in the cache.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="key">cache key</param>
        /// <returns></returns>
        Task<bool> Remove<T>(string key) where T : EntityModel;

        /// <summary>
        /// Removes a given <see cref="EntityModel"/> instance from the cache
        /// by given key <see cref="string"/>.<br />
        /// Returns true when the key was present in the cache.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="id">cache key</param>
        /// <returns></returns>
        Task<bool> Remove<T>(Guid id) where T : EntityModel;

        /// <summary>
        /// Refreshes a given <see cref="EntityModel"/> instance in the cache
        /// by given key <see cref="string"/> for the given 
        /// <see cref="TimeSpan"/>.<br />
        /// Returns true when the key was present in the cache.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="key">cache key</param>
        /// <param name="expires">expiration time span</param>
        /// <returns></returns>
        Task<bool> Refresh<T>(string key, TimeSpan expires) where T : EntityModel;

        /// <summary>
        /// Refreshes a given <see cref="EntityModel"/> instance in the cache
        /// by given key <see cref="Guid"/> for the given 
        /// <see cref="TimeSpan"/>.<br />
        /// Returns true when the key was present in the cache.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="id">cache key</param>
        /// <param name="expires">expiration time span</param>
        /// <returns></returns>
        Task<bool> Refresh<T>(Guid id, TimeSpan expires) where T : EntityModel;
    }
}
