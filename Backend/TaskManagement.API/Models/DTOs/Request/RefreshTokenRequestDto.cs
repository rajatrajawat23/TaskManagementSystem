using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.DTOs.Request
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}