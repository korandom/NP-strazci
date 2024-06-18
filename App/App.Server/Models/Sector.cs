namespace App.Server.Models
{
    public class Sector
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Color?
        public int DistrictId { get; set; }
        public District District { get; set; }
        public ICollection<Route> Routes { get;} = new List<Route>();
    }
}
