using App.Server.Models.AppData;
using System.ComponentModel.DataAnnotations;

namespace App.Server.DTOs
{
    public class AttendenceDto
    {
        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public RangerDto Ranger { get; set; }

        public bool Working { get; set; }

        public TimeOnly? From { get; set; }

        public ReasonOfAbsence ReasonOfAbsence { get; set; }
    }

    public class LockDto
    {
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        public int DistrictId { get; set; }
    }
}
