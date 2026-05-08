using Microsoft.AspNetCore.Identity;

namespace Lab04.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Address { get; set; }
    }
}
