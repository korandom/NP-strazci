using App.Server.DTOs;

namespace App.Server.Models.AppData
{

    public class Ranger
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ICollection<Plan> Plans { get; } = [];
        public ICollection<Attendence> Attendences { get; } = [];
        public District District { get; set; } = new District();
        public int DistrictId { get; set; }
    }

    public static class RangerExtensions
    {
        public static RangerDto ToDto(this Ranger ranger)
        {
            return new RangerDto
            {
                Id = ranger.Id,
                FirstName = ranger.FirstName,
                LastName = ranger.LastName,
                Email = ranger.Email,
                DistrictId = ranger.DistrictId
            };
        }
    }
}
