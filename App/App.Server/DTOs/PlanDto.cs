using App.Server.Models;

namespace App.Server.DTOs
{
    public class PlanDto
    {
        public DateOnly Date { get; set; }
        public int RangerId { get; set; }
        public ICollection<RouteDto> Routes { get; set; }
        public ICollection<VehicleDto> Vehicles { get; set; }

        public bool Locked { get; set; }
    }
}
