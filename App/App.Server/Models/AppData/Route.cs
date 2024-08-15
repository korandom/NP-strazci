using App.Server.DTOs;
using Microsoft.EntityFrameworkCore;

namespace App.Server.Models.AppData
{
    public class Route
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public ControlPlace? ControlPlace { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; }
        public ICollection<Plan> Plans { get; } = new List<Plan>();
    }

    [Owned]
    public class ControlPlace 
    {
        public string ControlTime { get; set; }
        public string ControlPlaceDescription { get; set; }
    }
    public static class RouteExtensions
    {
        public static RouteDto ToDto(this Route route)
        {
            return new RouteDto
            {
                Id = route.Id,
                Name = route.Name,
                Priority = route.Priority,
                ControlPlace = route.ControlPlace,
                DistrictId = route.DistrictId,
            };
        }
    }
}
