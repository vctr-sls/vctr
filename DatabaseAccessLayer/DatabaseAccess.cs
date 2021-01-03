using DatabaseAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DatabaseAccessLayer
{
    public class DatabaseAccess : IDatabaseAccess
    {
        private readonly DatabaseContext ctx;

        public DatabaseAccess(DatabaseContext _ctx) => ctx = _ctx;

        public Task<long> GetCount<T>() where T : EntityModel =>
            GetTable<T>().LongCountAsync();

        public async Task<T> GetById<T>(Guid id) where T : EntityModel =>
            await ctx.FindAsync<T>(id);

        //public async Task<T> GetById<T>(Guid id) where T : EntityModel =>
        //    await GetTable<T>().FirstOrDefaultAsync(e => e.Guid == id);

        public IQueryable<T> GetAll<T>() where T : EntityModel =>
            GetTable<T>();

        public IQueryable<T> GetWhere<T>(Expression<Func<T, bool>> preticate) where T : EntityModel =>
            GetTable<T>().Where(preticate);

        public T Create<T>(T model) where T : EntityModel =>
            ctx.Add(model) as T;

        public T Update<T>(T model) where T : EntityModel =>
            ctx.Update(model).Entity;

        public T Delete<T>(T model) where T : EntityModel =>
            GetTable<T>().Remove(model).Entity;

        public void DeleteRange<T>(params T[] model) where T : EntityModel =>
            GetTable<T>().RemoveRange(model);

        public Task Commit() => ctx.SaveChangesAsync();

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
    }
}
