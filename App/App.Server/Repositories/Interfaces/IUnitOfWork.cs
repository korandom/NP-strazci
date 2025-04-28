using App.Server.Models.AppData;

namespace App.Server.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        public IGenericRepository<District> DistrictRepository { get; }

        public IGenericRepository<Models.AppData.Route> RouteRepository { get; }

        public IGenericRepository<Vehicle> VehicleRepository { get; }

        public IGenericRepository<Ranger> RangerRepository { get; }
        public IGenericRepository<Models.AppData.Lock> LockRepository { get; }
        public IGenericRepository<Plan> PlanRepository { get; }

        public IGenericRepository<Attendence> AttendenceRepository { get; }
        public Task SaveAsync();
    }
}
