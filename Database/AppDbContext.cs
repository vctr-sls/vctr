using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using slms2asp.Models;

namespace slms2asp.Database
{
    /// <summary>
    /// 
    /// Application database context.
    /// 
    /// </summary>
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration Configuration;

        public DbSet<ShortLinkModel> ShortLinks { get; private set; }
        public DbSet<AccessModel> Accesses { get; private set; }
        public DbSet<GeneralSettingsModel> GeneralSettings { get; private set; }

        public AppDbContext() { }

        public AppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = Configuration.GetConnectionString("MySQL") ?? "";

            optionsBuilder.UseMySql(connectionString);
        }
    }
}
