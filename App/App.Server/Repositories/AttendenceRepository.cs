using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace App.Server.Repositories
{
    /// <summary>
    /// AttendenceRepository is a specialized repository for mapping Attendence to the database, with basic function.
    /// It implements the IGenericRepository Interface by extending the GenericRepository<Attendence>.
    /// Only the GetById function is overriden, since attendence has a composite key.
    /// </summary>
    /// <param name="context"> The database context for the app. </param>
    public class AttendenceRepository(PlannerNPContext context) : GenericRepository<Attendence>(context), IGenericRepository<Attendence>
    {

        // Accepts an array with first being Date and second being RangerId
        public override async Task<Attendence?> GetById(params object[] id)
        {
            if (id.Length != 2) return null;
            return await _dbSet.Include(attend => attend.Ranger)
                               .FirstOrDefaultAsync(attend => attend.Date == (DateOnly)id[0] && attend.RangerId == (int)id[1]);
        }
    }
}
