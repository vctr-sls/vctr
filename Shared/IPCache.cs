using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;

namespace slms2asp.Shared
{
    public class IPCacheEntry
    {
        public DateTime Expires;
        public Guid Guid;

        public bool IsExpired() =>
            DateTime.Now.CompareTo(Expires) >= 0; 
    }

    public class IPCache
    {
        private readonly Dictionary<IPAddress, IPCacheEntry> Entries;
        private readonly Timer CleanupTimer;
        private readonly double CleanupInterval;
        private readonly TimeSpan Expiration;

        public IPCache(TimeSpan cleanupInterval, TimeSpan expiration)
        {
            Entries = new Dictionary<IPAddress, IPCacheEntry>();
            CleanupInterval = cleanupInterval.TotalMilliseconds;
            Expiration = expiration;

            CleanupTimer = new Timer(CleanupInterval);
            CleanupTimer.Elapsed += OnCleanup;
            CleanupTimer.Start();
        }

        public void Push(IPAddress address, Guid guid)
        {
            Entries.Add(address, new IPCacheEntry()
            {
                Guid = guid,
                Expires = DateTime.Now.Add(Expiration),
            });
        }

        public Guid? Get(IPAddress address)
        {
            IPCacheEntry entry;

            try
            {
                entry = Entries[address];
            }
            catch (Exception)
            {
                return null;
            }

            if (entry.IsExpired())
            {
                Entries.Remove(address);
                return null;
            }

            return entry?.Guid;
        }

        public bool Contains(IPAddress address)
        {
            if (!Entries.ContainsKey(address))
            {
                return false;
            }

            if (Get(address) == null)
            {
                return false;
            }

            return true;
        }

        private void OnCleanup(Object source, ElapsedEventArgs e)
        {
            foreach (KeyValuePair<IPAddress, IPCacheEntry> kv in Entries)
            {
                if (kv.Value.IsExpired())
                {
                    Entries.Remove(kv.Key);
                }
            }
        }
    }
}
