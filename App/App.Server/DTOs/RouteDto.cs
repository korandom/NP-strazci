using App.Server.Models;

namespace App.Server.DTOs
{
    public class RouteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public ControlPlace? ControlPlace { get; set; }
        public int DistrictId { get; set; }
    }
}
