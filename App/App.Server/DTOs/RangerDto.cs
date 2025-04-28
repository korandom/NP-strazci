using System.ComponentModel.DataAnnotations;

namespace App.Server.DTOs
{
    public class RangerDto
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        public int DistrictId { get; set; }
    }
}
