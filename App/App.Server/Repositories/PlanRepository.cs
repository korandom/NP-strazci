using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace App.Server.Repositories
{
    /// <summary>
    /// PlanRepository is a specialized repository for mapping Plan to the database, with basic function.
    /// It implements the IGenericRepository Interface by extending the GenericRepository<Plan>.
    /// Only the GetById function is overriden, since plan has a composite key.
    /// </summary>
    /// <param name="context"> The database context for the app. </param>
    public class PlanRepository(PlannerNPContext context) : GenericRepository<Plan>(context), IGenericRepository<Plan>
    {

        // Accepts an array with first being Date and second being RangerId
        public override async Task<Plan?> GetById(params object[] id)
        {
            if (id.Length != 2) return null;
            return await _dbSet.Include(plan => plan.Routes)
                               .Include(plan => plan.Vehicles)
                               .Include(plan => plan.Ranger)
                               .FirstOrDefaultAsync(plan => plan.Date == (DateOnly)id[0] && plan.RangerId == (int)id[1]);
        }
    }
}
