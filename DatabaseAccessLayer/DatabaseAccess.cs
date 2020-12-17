using DatabaseAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccessLayer
{
    public class DatabaseAccess : IDatabaseAccess
    {
        private readonly DatabaseContext ctx;

        public DatabaseAccess(DatabaseContext _ctx) => ctx = _ctx;

        public Task<long> GetCount<T>() where T : EntityModel =>
            GetTable<T>().LongCountAsync();

        public async Task<T> Create<T>(T model) where T : EntityModel =>
            await ctx.AddAsync(model) as T;

        public async Task<T> GetById<T>(Guid id) where T : EntityModel =>
            await ctx.FindAsync<T>(id);

        private DbSet<T> GetTable<T>() where T : EntityModel
        {
            var typ = typeof(T);
            if (typ == typeof(UserModel))
                return ctx.Users as DbSet<T>;
            else if (typ == typeof(LinkModel))
                return ctx.Links as DbSet<T>;
            else if (typ == typeof(AccessModel))
                return ctx.Accesses as DbSet<T>;
            else
                throw new ArgumentException("invalid entity type");
        }

        public Task Commit() => ctx.SaveChangesAsync();
    }
}
