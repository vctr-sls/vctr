using slms2asp.Database;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace slms2asp.Shared
{

    public class AppDbCache
    {
        private readonly AppDbContext Db;
        private readonly ConcurrentDictionary<Guid, List<AccessModel>> AccessMap;
        private readonly Timer Timer;

        public AppDbCache(AppDbContext db, TimeSpan timerInterval)
        {
            Db = db;
            AccessMap = new ConcurrentDictionary<Guid, List<AccessModel>>();
            Timer = new Timer(timerInterval.TotalMilliseconds);

            Timer.Elapsed += async (object s, ElapsedEventArgs e) =>
            {
                await PushToDB();
            };

            Timer.Start();
        }

        public List<AccessModel> GetAccesses(Guid guid)
        {
            List<AccessModel> accesses;

            if (!AccessMap.ContainsKey(guid))
            {
                accesses = Db.Accesses.Where(a => a.ShortLinkGUID == guid).ToList();
                AccessMap.AddOrUpdate(guid, accesses, (k, v) => v);
            }
            else
            {
                accesses = AccessMap[guid];
            }

            return accesses;
        }

        public void AddAccess(AccessModel access)
        {
            access.IsNewEntry = true;
            var accesses = GetAccesses(access.ShortLinkGUID);
            accesses.Add(access);
        }

        public async Task PushToDB()
        {
            var newEntries = new List<AccessModel>();

            AccessMap.Values.ToList().ForEach(accesses =>
            {
                newEntries.AddRange(accesses.Where(a => a.IsNewEntry));
            });

            if (newEntries.Count > 0)
            {
                Db.Accesses.AddRange(newEntries);
                await Db.SaveChangesAsync();
                newEntries.ForEach(e => e.IsNewEntry = false);
            }
        }
    }
}
