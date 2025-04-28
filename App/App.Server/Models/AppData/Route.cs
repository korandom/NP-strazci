using App.Server.DTOs;
using Microsoft.EntityFrameworkCore;

namespace App.Server.Models.AppData
{
    public class Route
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Priority { get; set; }
        public ControlPlace? ControlPlace { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; } = new District();
        public ICollection<Plan> Plans { get; } = [];
    }

    [Owned]
    public class ControlPlace
    {
        public string ControlTime { get; set; } = string.Empty;
        public string ControlPlaceDescription { get; set; } = string.Empty;
    }
    public static class RouteExtensions
    {
        public static RouteDto ToDto(this Route route)
        {
            return new RouteDto(route.Id, route.Name, route.Priority, route.ControlPlace, route.DistrictId);
        }
    }
}
