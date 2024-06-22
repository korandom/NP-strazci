using App.Server.Models;
using App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace App.Server.Repositories
{
    public class PlanRepository : GenericRepository<Plan>, IGenericRepository<Plan>
    {

        public PlanRepository(PlannerNPContext context) :base(context) { }

        // Accepts an array with first being Date and second being RangerId
        public override async Task<Plan?> GetById(params object[] id)
        {
            if (id.Length != 2) return null;
            return await _dbSet.Include(plan => plan.Routes)
                               .Include(plan => plan.Vehicles)
                               .FirstOrDefaultAsync(plan => plan.Date == (DateOnly)id[0] && plan.RangerId == (int)id[1]);
        }
    }
}
