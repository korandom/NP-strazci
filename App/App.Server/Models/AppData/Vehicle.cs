using App.Server.DTOs;

namespace App.Server.Models.AppData
{

    public class Vehicle
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }

        public int DistrictId { get; set; }
        public ICollection<Plan> Plans { get; } = [];
    }

    public static class VehicleExtensions
    {
        public static VehicleDto ToDto(this Vehicle vehicle)
        {
            return new VehicleDto
            {
                Id = vehicle.Id,
                Name = vehicle.Name,
                Type = vehicle.Type,
                DistrictId = vehicle.DistrictId,
            };
        }
    }
}
