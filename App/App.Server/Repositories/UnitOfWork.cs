using App.Server.Models;
using App.Server.Repositories.Interfaces;

namespace App.Server.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private PlannerNPContext _context;
        private GenericRepository<District>? districtRepository;
        private GenericRepository<Sector>? sectorRepository;
        private GenericRepository<Models.Route>? routeRepository;
        private GenericRepository<Vehicle>? vehicleRepository;
        private GenericRepository<Ranger>? rangerRepository;
        private PlanRepository? planRepository;

        public UnitOfWork(PlannerNPContext context)
        {
            _context = context;
        }

        public GenericRepository<District> DistrictRepository
        {
            get
            {
                if (this.districtRepository == null)
                {
                    this.districtRepository = new GenericRepository<District>(_context);
                }
                return districtRepository;
            }
        }

        public GenericRepository<Sector> SectorRepository
        {
            get
            {
                if (this.sectorRepository == null)
                {
                    this.sectorRepository = new GenericRepository<Sector>(_context);
                }
                return sectorRepository;
            }
        }

        public GenericRepository<Models.Route> RouteRepository
        {
            get 
            {
                if (this.routeRepository == null)
                {
                    this.routeRepository = new GenericRepository<Models.Route>(_context);
                }
                return routeRepository;
            }
        }

        public GenericRepository<Vehicle> VehicleRepository
        {
            get
            {
                if (this.vehicleRepository == null)
                {
                    this.vehicleRepository = new GenericRepository<Vehicle>(_context);
                }
                return vehicleRepository;
            }
        }

        public GenericRepository<Ranger> RangerRepository
        {
            get
            {
                if (this.rangerRepository == null)
                {
                    this.rangerRepository = new GenericRepository<Ranger>(_context);
                }
                return rangerRepository;
            }
        }

        public PlanRepository PlanRepository
        {
            get
            {
                if (this.planRepository == null)
                {
                    this.planRepository = new PlanRepository(_context);
                }
                return planRepository;
            }
        }
        public async Task SaveAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
