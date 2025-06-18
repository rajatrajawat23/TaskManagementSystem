using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.DTOs.Request
{
    public class RegisterRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public Guid? CompanyId { get; set; }

        [Required]
        public string Role { get; set; }

        public string Department { get; set; }

        public string JobTitle { get; set; }
    }
}