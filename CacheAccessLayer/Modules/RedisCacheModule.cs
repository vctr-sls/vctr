using DatabaseAccessLayer.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace CacheAccessLayer.Modules
{
    public class RedisCacheModule : ICacheAccess
    {
        private readonly ConnectionMultiplexer connections;
        private readonly IDatabase database;

        public RedisCacheModule(IConfiguration config)
        {
            var servers = config.GetValue<string>("Caching:Redis:Servers");
            var databaseNumber = config.GetValue("Caching:Redis:Database", 0);
            connections = ConnectionMultiplexer.Connect(servers);
            database = connections.GetDatabase(databaseNumber);
        }

        public Task<(bool Ok, T Value)> Get<T>(Guid id) where T : EntityModel =>
            Get<T>(id.ToString());

        public async Task<(bool Ok, T Value)> Get<T>(string key) where T : EntityModel
        {
            var resStr = await database.StringGetWithExpiryAsync(Key<T>(key));
            if (resStr.Value.IsNullOrEmpty)
                return (false, null);
            var res = Deserialize<T>(resStr.Value);
            return (true, res);
        }

        public Task<bool> Remove<T>(Guid id) where T : EntityModel =>
            Remove<T>(id.ToString());

        public Task<bool> Remove<T>(string key) where T : EntityModel =>
            database.KeyDeleteAsync(Key<T>(key));

        public Task<bool> Set<T>(T model, TimeSpan expires) where T : EntityModel =>
            Set<T>(model, model.Guid.ToString(), expires);

        public async Task<bool> Set<T>(T model, string key, TimeSpan expires) where T : EntityModel
        {
            key = Key<T>(key);
            if (await database.StringSetAsync(key, Serialize(model)))
                return await database.KeyExpireAsync(key, expires);
            return false;
        }

        public Task<bool> Refresh<T>(Guid id, TimeSpan expires) where T : EntityModel =>
            Refresh<T>(id.ToString(), expires);

        public async Task<bool> Refresh<T>(string key, TimeSpan expires) where T : EntityModel =>
            await database.KeyExpireAsync(Key<T>(key), expires);

        private string Key<T>(T model) where T : EntityModel =>
            Key<T>(model.Guid.ToString());

        private string Key<T>(string key) where T : EntityModel =>
            $"{typeof(T).FullName}-{key}";

        private string Serialize<T>(T model) where T : EntityModel =>
            JsonConvert.SerializeObject(model);

        private T Deserialize<T>(string data) where T : EntityModel =>
            JsonConvert.DeserializeObject<T>(data);
    }
}
