using App.Server.Models;

namespace App.Server.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        public GenericRepository<District> DistrictRepository { get; }

        public GenericRepository<Models.Route> RouteRepository { get; }

        public GenericRepository<Vehicle> VehicleRepository { get; }

        public GenericRepository<Ranger> RangerRepository { get; }

        public PlanRepository PlanRepository { get; }
        public Task SaveAsync();
    }
}
