using Microsoft.EntityFrameworkCore;

namespace App.Server.Models
{
    [PrimaryKey(nameof(Date), nameof(RangerId))]
    public class Plan
    {
        public DateOnly Date { get; set; }
        public int RangerId { get; set; }
        public Ranger Ranger { get; set; }

        public ICollection<Route> Routes { get; } = new List<Route>();
        public ICollection<Vehicle> Vehicles { get; } = new List<Vehicle>();
        public int Locked {  get; set; }
    }
}
