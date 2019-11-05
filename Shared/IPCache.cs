using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;

namespace slms2asp.Shared
{
    /// <summary>
    /// 
    /// Entry wrapper for IPCache containing the
    /// actual value and the date of expiration.
    /// 
    /// </summary>
    public class IPCacheEntry
    {
        public DateTime Expires;
        public Guid Guid;

        /// <summary>
        /// 
        /// Returns true if the expiration date is
        /// after the date of now.
        /// 
        /// </summary>
        /// <returns>is expired state</returns>
        public bool IsExpired() =>
            DateTime.Now.CompareTo(Expires) >= 0; 
    }

    /// <summary>
    /// 
    /// Manages a dictionary of IPAddresses with set values
    /// which will expire and be deleted from the cache
    /// after a defined time span.
    /// 
    /// On initialization, a timer wil the set elapse duration
    /// will be started which looks for expired values and
    /// deletes them from the cache.
    /// 
    /// </summary>
    public class IPCache
    {
        private readonly Dictionary<IPAddress, IPCacheEntry> Entries;
        private readonly Timer CleanupTimer;
        private readonly double CleanupInterval;
        private readonly TimeSpan Expiration;

        /// <summary>
        /// 
        /// Initializes a new instance of IPCache and
        /// starts the cleanup timer.
        /// 
        /// </summary>
        /// <param name="cleanupInterval">
        ///     time to elapse until next cleanup
        /// </param>
        /// <param name="expiration">
        ///     time span after registration until entry 
        ///     will be marked as expired
        /// </param>
        public IPCache(TimeSpan cleanupInterval, TimeSpan expiration)
        {
            Entries = new Dictionary<IPAddress, IPCacheEntry>();
            CleanupInterval = cleanupInterval.TotalMilliseconds;
            Expiration = expiration;

            CleanupTimer = new Timer(CleanupInterval);
            CleanupTimer.Elapsed += OnCleanup;
            CleanupTimer.Start();
        }

        /// <summary>
        /// 
        /// Pushes a new entry to the cache.
        /// 
        /// </summary>
        /// <param name="address">request IP Address</param>
        /// <param name="guid">GUID</param>
        public void Push(IPAddress address, Guid guid)
        {
            Entries.Add(address, new IPCacheEntry()
            {
                Guid = guid,
                Expires = DateTime.Now.Add(Expiration),
            });
        }

        /// <summary>
        /// 
        /// Returns a saved value by its IP Address
        /// if the value is existent and if the value
        /// is not expired.
        /// 
        /// If no matching value could be found or if
        /// the value is expired, <i>null</i> will be returned
        /// and the value will be deleted from the 
        /// dictionary, if existent.
        /// 
        /// </summary>
        /// <param name="address">IP request addredd</param>
        /// <returns>GUID value or null</returns>
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

        /// <summary>
        /// 
        /// Returns if the cache contains a valid
        /// value. This will also delete values, 
        /// which are maked as expired and existent
        /// in the cache.
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// Iterates through all entries and deletes
        /// those which are marked as expired.
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
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
