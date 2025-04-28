using App.Server.DTOs;

namespace App.Server.Models.AppData
{
    public class District
    {
        public int Id { get; set; }

        public ICollection<Ranger> Rangers { get; } = [];
        public ICollection<Vehicle> Vehicles { get; } = [];
        public string Name { get; set; } = string.Empty;
    }

    public static class DistrictExtensions
    {
        public static DistrictDto ToDto(this District district)
        {
            return new DistrictDto
            {
                Id = district.Id,
                Name = district.Name
            };
        }
    }
}
