using App.Server.Models.AppData;

namespace App.Server.DTOs
{
    public class RouteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public ControlPlace? ControlPlace { get; set; }
        public int DistrictId { get; set; }

        public RouteDto(int id, string name, int priority, ControlPlace? controlPlace, int districtId)
        {
            Id = id;
            Name = name;
            Priority = priority;
            ControlPlace = controlPlace;
            DistrictId = districtId;
        }
    }
}
