using App.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace App.Server.DTOs
{
    public class PlanDto
    {
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        public RangerDto Ranger { get; set; }
        public int[] RouteIds { get; set; } = [];
        public int[] VehicleIds { get; set; } = [];

        public PlanDto(DateOnly date, RangerDto ranger, int[] routeIds, int[] vehicleIds)
        {
            Date = date;
            Ranger = ranger;
            RouteIds = routeIds;
            VehicleIds = vehicleIds;
        }
    }

}
