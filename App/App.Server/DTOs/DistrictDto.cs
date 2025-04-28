using System.ComponentModel.DataAnnotations;

namespace App.Server.DTOs
{
    public class DistrictDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
