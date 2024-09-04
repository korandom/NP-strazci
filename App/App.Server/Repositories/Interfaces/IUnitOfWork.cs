using App.Server.Models.AppData;

namespace App.Server.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        public GenericRepository<District> DistrictRepository { get; }

        public GenericRepository<Models.AppData.Route> RouteRepository { get; }

        public GenericRepository<Vehicle> VehicleRepository { get; }

        public GenericRepository<Ranger> RangerRepository { get; }
        public GenericRepository<Lock> LockRepository { get; }
        public PlanRepository PlanRepository { get; }
        public Task SaveAsync();
    }
}
