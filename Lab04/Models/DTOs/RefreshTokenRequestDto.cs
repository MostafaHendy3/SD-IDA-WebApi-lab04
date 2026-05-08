using System.ComponentModel.DataAnnotations;

namespace Lab04.Models.DTOs
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
