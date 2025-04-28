using System.ComponentModel.DataAnnotations;

namespace App.Server.DTOs
{
    public class UserDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Role { get; set; }
        public int? RangerId { get; set; }
    }
}
