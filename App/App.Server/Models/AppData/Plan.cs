using App.Server.DTOs;
using Microsoft.EntityFrameworkCore;

namespace App.Server.Models.AppData
{
    [PrimaryKey(nameof(Date), nameof(RangerId))]
    public class Plan
    {
        public DateOnly Date { get; set; }
        public int RangerId { get; set; }
        public Ranger Ranger { get; set; }

        public ICollection<Route> Routes { get; } = new List<Route>();
        public ICollection<Vehicle> Vehicles { get; } = new List<Vehicle>();

        public Plan() { }
        public Plan(DateOnly date, Ranger ranger)
        {
            Date = date;
            RangerId = ranger.Id;
            Ranger = ranger;
        }
    }

    public static class PlanExtensions
    {
        public static PlanDto ToDto(this Plan plan)
        {
            return new PlanDto
            {
                Date = plan.Date,
                Ranger = plan.Ranger.ToDto(),
                RouteIds = plan.Routes.Select(r => r.Id).ToArray(),
                VehicleIds = plan.Vehicles.Select(v => v.Id).ToArray()
            };
        }
    }
}
