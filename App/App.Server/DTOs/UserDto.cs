namespace App.Server.DTOs
{
    public class UserDto
    {
        public required string Email { get; set; }
        public required string Role { get; set; }
        public int? RangerId { get; set; }
    }
}
