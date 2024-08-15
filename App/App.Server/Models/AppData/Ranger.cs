using App.Server.DTOs;
using Microsoft.EntityFrameworkCore;

namespace App.Server.Models.AppData
{

    public class Ranger
    {   
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public ICollection<Plan> Plans { get; } = new List<Plan>();
        public District District { get; set; }
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
