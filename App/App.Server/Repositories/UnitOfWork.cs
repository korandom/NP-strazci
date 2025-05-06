using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;

namespace App.Server.Repositories
{
    /// <summary>
    /// Implements the Unit of Work pattern to provide repositories.
    /// Provides a centralized point for committing data changes.
    /// </summary>
    public class UnitOfWork(PlannerNPContext context) : IUnitOfWork
    {
        private readonly PlannerNPContext _context = context;
        private GenericRepository<District>? districtRepository;
        private GenericRepository<Models.AppData.Route>? routeRepository;
        private GenericRepository<Vehicle>? vehicleRepository;
        private GenericRepository<Ranger>? rangerRepository;
        private GenericRepository<Models.AppData.Lock>? lockRepository;
        private PlanRepository? planRepository;
        private AttendenceRepository? attendenceRepository;

        public IGenericRepository<District> DistrictRepository
        {
            get
            {
                this.districtRepository ??= new GenericRepository<District>(_context);
                return districtRepository;
            }
        }

        public IGenericRepository<Models.AppData.Route> RouteRepository
        {
            get
            {
                this.routeRepository ??= new GenericRepository<Models.AppData.Route>(_context);
                return routeRepository;
            }
        }

        public IGenericRepository<Vehicle> VehicleRepository
        {
            get
            {
                this.vehicleRepository ??= new GenericRepository<Vehicle>(_context);
                return vehicleRepository;
            }
        }

        public IGenericRepository<Ranger> RangerRepository
        {
            get
            {
                this.rangerRepository ??= new GenericRepository<Ranger>(_context);
                return rangerRepository;
            }
        }

        public IGenericRepository<Models.AppData.Lock> LockRepository
        {
            get
            {
                this.lockRepository ??= new GenericRepository<Models.AppData.Lock>(_context);
                return lockRepository;
            }
        }

        public IGenericRepository<Plan> PlanRepository
        {
            get
            {
                this.planRepository ??= new PlanRepository(_context);
                return planRepository;
            }
        }

        public IGenericRepository<Attendence> AttendenceRepository
        {
            get
            {
                this.attendenceRepository ??= new AttendenceRepository(_context);
                return attendenceRepository;
            }
        }

        /// <summary>
        /// Persists all changes made through the repositories to the database.
        /// </summary>
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
