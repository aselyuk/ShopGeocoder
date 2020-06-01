using Microsoft.EntityFrameworkCore;
using ShopGeocoder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopGeocoder.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Shop> Shops { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<Shop>().Property(p => p.Guid).HasDefaultValueSql("uuid_generate_v4()");
            modelBuilder.Entity<ApiKey>().Property(p => p.Guid).HasDefaultValueSql("uuid_generate_v4()");

            base.OnModelCreating(modelBuilder);
        }
    }
}
