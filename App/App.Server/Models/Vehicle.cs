namespace App.Server.Models
{

    public class Vehicle
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
        public ICollection<Plan> Plans { get; } = new List<Plan>();
    }
}
