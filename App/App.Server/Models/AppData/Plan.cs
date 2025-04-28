using App.Server.DTOs;
using Microsoft.EntityFrameworkCore;

namespace App.Server.Models.AppData
{
    [PrimaryKey(nameof(Date), nameof(RangerId))]
    public class Plan
    {
        public DateOnly Date { get; set; }
        public int RangerId { get; set; }
        public Ranger Ranger { get; set; } = new Ranger();

        public ICollection<Route> Routes { get; set; } = [];
        public ICollection<Vehicle> Vehicles { get; set; } = [];

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
            return new PlanDto(
                plan.Date,
                plan.Ranger.ToDto(),
                plan.Routes.Select(r => r.Id).ToArray(),
                plan.Vehicles.Select(v => v.Id).ToArray()
            );
        }
    }
}
