using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace App.Server.Models.AppData
{
    public class PlannerNPContext(DbContextOptions<PlannerNPContext> options) : DbContext(options)
    {
        public DbSet<Ranger>  Rangers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<District> Districts { get; set; }

        public DbSet<Attendence> Attendences { get; set; }

        public DbSet<Lock> Locks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
        }

    }
}
