namespace App.Server.Models
{
    public class District
    {
        public int Id { get; set; }

        public ICollection<Ranger> Rangers { get; } = new List<Ranger>();
        public string? Name { get; set; }
        public ICollection<Sector> Sectors { get; } = new List<Sector>();
    }
}
