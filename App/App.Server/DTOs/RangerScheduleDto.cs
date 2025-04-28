using App.Server.Models.AppData;
using System.ComponentModel.DataAnnotations;

namespace App.Server.DTOs
{
    public class RangerScheduleDto
    {
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        public RangerDto Ranger { get; set; }
        [Required]
        public bool Working { get; set; }

        public TimeOnly? From { get; set; }

        public ReasonOfAbsence ReasonOfAbsence { get; set; }

        public int[] RouteIds { get; set; } = [];
        public int[] VehicleIds { get; set; } = [];
    }
}
