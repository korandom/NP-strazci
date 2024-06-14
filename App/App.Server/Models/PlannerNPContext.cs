using Microsoft.EntityFrameworkCore;

namespace App.Server.Models
{
    public class PlannerNPContext : DbContext
    {
        public DbSet<Ranger> Rangers { get; set; }

        public PlannerNPContext(DbContextOptions options) : base(options) {}
    }
}
