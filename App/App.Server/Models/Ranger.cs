using Microsoft.EntityFrameworkCore;

namespace App.Server.Models
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
}
