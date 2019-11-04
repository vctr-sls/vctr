using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using slms2asp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slms2asp.Database
{
    /// <summary>
    /// 
    /// Application database context.
    /// 
    /// </summary>
    public class AppDbContext : DbContext
    {
        private IConfiguration configuration;

        public DbSet<ShortLinkModel> ShortLinks { get; private set; }
        public DbSet<AccessModel> Accesses { get; private set; }
        public DbSet<GeneralSettingsModel> GeneralSettings { get; private set; }

        public AppDbContext() { }

        public AppDbContext(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = configuration.GetConnectionString("MySQL") ?? "";

            optionsBuilder.UseMySql(connectionString);
        }
    }
}
