using App.Server.Models;

namespace App.Server.DTOs
{
    public class PlanDto
    {
        public DateOnly Date { get; set; }
        public RangerDto Ranger { get; set; }
        public int[] RouteIds { get; set; }
        public int[] VehicleIds { get; set; }
    }

    public class LockDto
    {
        public DateOnly Date { get; set; }
        public int DistrictId { get; set; }
    }
}
