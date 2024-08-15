using App.Server.Models.AppData;
using Microsoft.AspNetCore.Identity;

namespace App.Server.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public int? RangerId { get; set; }
    }
}
