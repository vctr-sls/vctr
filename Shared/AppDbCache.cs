using slms2asp.Database;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace slms2asp.Shared
{
    public class CacheEntry<T>
    {
        public T Element;
        public DateTime Expires;

        public CacheEntry(T element, TimeSpan expires) 
        {
            Element = element;
            Expires = DateTime.Now.Add(expires);
        }

        public bool IsExpired() =>
            DateTime.Now.CompareTo(Expires) >= 0;
    }

    public class AppDbCache
    {
        private readonly AppDbContext Db;
        private readonly ConcurrentDictionary<Guid, CacheEntry<List<AccessModel>>> AccessMap;
        private readonly Timer Timer;
        private readonly TimeSpan Expiring;

        public AppDbCache(AppDbContext db, TimeSpan timerInterval, TimeSpan expiring)
        {
            Db = db;
            AccessMap = new ConcurrentDictionary<Guid, CacheEntry<List<AccessModel>>>();
            Timer = new Timer(timerInterval.TotalMilliseconds);
            Expiring = expiring;

            Timer.Elapsed += async (object s, ElapsedEventArgs e) =>
            {
                await PushToDB();
                CleanUp();
            };

            Timer.Start();
        }

        public AccessModel[] GetAccesses(Guid guid) =>
            GetAccessEntry(guid).Element.ToArray();

        public void AddAccess(AccessModel access)
        {
            access.IsNewEntry = true;
            var accesses = GetAccessEntry(access.ShortLinkGUID);
            accesses.Element.Add(access);
        }

        private async Task PushToDB()
        {
            var newEntries = new List<AccessModel>();

            AccessMap.Values.ToList().ForEach(accesses =>
            {
                newEntries.AddRange(accesses.Element.Where(a => a.IsNewEntry));
            });

            if (newEntries.Count > 0)
            {
                Db.Accesses.AddRange(newEntries);
                await Db.SaveChangesAsync();
                newEntries.ForEach(e => e.IsNewEntry = false);
            }
        }

        private void CleanUp()
        {
            AccessMap.Keys.ToList().ForEach(key =>
            {
                if (AccessMap[key].IsExpired())
                {
                    CacheEntry<List<AccessModel>> entry;
                    AccessMap.Remove(key, out entry);
                }
            });
        }

        private CacheEntry<List<AccessModel>> GetAccessEntry(Guid guid)
        {
            CacheEntry<List<AccessModel>> accessEntry;

            if (!AccessMap.ContainsKey(guid))
            {
                var accesses = Db.Accesses.Where(a => a.ShortLinkGUID == guid).ToList();
                accessEntry = new CacheEntry<List<AccessModel>>(accesses, Expiring);
                AccessMap.AddOrUpdate(guid, accessEntry, (k, v) => v);
            }
            else
            {
                accessEntry = AccessMap[guid];
            }

            return accessEntry;
        }
    }
}
