using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;

namespace App.Server.Repositories
{
    public class UnitOfWork(PlannerNPContext context) : IUnitOfWork
    {
        private PlannerNPContext _context = context;
        private GenericRepository<District>? districtRepository;
        private GenericRepository<Models.AppData.Route>? routeRepository;
        private GenericRepository<Vehicle>? vehicleRepository;
        private GenericRepository<Ranger>? rangerRepository;
        private GenericRepository<Lock>? lockRepository;
        private PlanRepository? planRepository;
        private AttendenceRepository? attendenceRepository;

        public GenericRepository<District> DistrictRepository
        {
            get
            {
                this.districtRepository ??= new GenericRepository<District>(_context);
                return districtRepository;
            }
        }

        public GenericRepository<Models.AppData.Route> RouteRepository
        {
            get 
            {
                this.routeRepository ??= new GenericRepository<Models.AppData.Route>(_context);
                return routeRepository;
            }
        }

        public GenericRepository<Vehicle> VehicleRepository
        {
            get
            {
                this.vehicleRepository ??= new GenericRepository<Vehicle>(_context);
                return vehicleRepository;
            }
        }

        public GenericRepository<Ranger> RangerRepository
        {
            get
            {
                this.rangerRepository ??= new GenericRepository<Ranger>(_context);
                return rangerRepository;
            }
        }

        public GenericRepository<Lock> LockRepository
        {
            get
            {
                this.lockRepository ??= new GenericRepository<Lock>(_context);
                return lockRepository;
            }
        }

        public PlanRepository PlanRepository
        {
            get
            {
                this.planRepository ??= new PlanRepository(_context);
                return planRepository;
            }
        }

        public AttendenceRepository AttendenceRepository
        {
            get
            {
                this.attendenceRepository ??= new AttendenceRepository(_context);
                return attendenceRepository;
            }
        }
        public async Task SaveAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
