using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace App.Server.Models
{
    public class PlannerNPContext : DbContext
    {
        public DbSet<Ranger>  Rangers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<District> Districts { get; set; }

        public PlannerNPContext(DbContextOptions options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
        }

    }
}
