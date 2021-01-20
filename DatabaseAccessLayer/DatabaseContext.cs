using DatabaseAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccessLayer
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UserModel> Users { get; private set; }
        public DbSet<LinkModel> Links { get; private set; }
        public DbSet<AccessModel> Accesses { get; private set; }
        public DbSet<ApiKeyModel> ApiKeys { get; private set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }
    }
}
