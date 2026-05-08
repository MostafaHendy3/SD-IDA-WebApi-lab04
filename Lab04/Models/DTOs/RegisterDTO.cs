using System.ComponentModel.DataAnnotations;

namespace Lab04.Models.DTOs
{
    public class RegisterDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }

        public string Address { get; set; }
    }
}
