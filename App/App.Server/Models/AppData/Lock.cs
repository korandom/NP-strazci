using Microsoft.EntityFrameworkCore;

namespace App.Server.Models.AppData
{
    [PrimaryKey(nameof(Date), nameof(DistrictId))]

    public class Lock
    {
        public DateOnly Date { get; set; }
        public District District { get; set; }
        public int DistrictId { get; set; }
    }
}
