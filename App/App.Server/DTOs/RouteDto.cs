using App.Server.Models.AppData;
using System.ComponentModel.DataAnnotations;

namespace App.Server.DTOs
{
    public class RouteDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0, 3)]
        public int Priority { get; set; }
        public ControlPlace? ControlPlace { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
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
