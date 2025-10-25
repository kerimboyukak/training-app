using DevOps.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOps.Infrastructure
{
    // DbSet<Team> Teams { get; set; }
    internal class DevOpsContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<Developer> Developers { get; set; }

        public DevOpsContext(DbContextOptions<DevOpsContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new DeveloperConfiguration().Configure(modelBuilder.Entity<Developer>());   //configures the entity in a separate class
            new TeamConfiguration().Configure(modelBuilder.Entity<Team>());
        }
    }
}
