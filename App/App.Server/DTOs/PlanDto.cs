using App.Server.Models;

namespace App.Server.DTOs
{
    public class PlanDto
    {

        public DateOnly Date { get; set; }
        public RangerDto Ranger { get; set; }
        public int[] RouteIds { get; set; } = new int[0];
        public int[] VehicleIds { get; set; } = new int[0];

    }

}
