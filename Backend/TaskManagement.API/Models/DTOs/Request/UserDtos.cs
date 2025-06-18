using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.DTOs.Request
{
    public class CreateUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; } = "User"; // CompanyAdmin, Manager, User, TaskAssigner

        [StringLength(50)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        // Set by system
        public Guid CompanyId { get; set; }
        public Guid CreatedById { get; set; }
    }

    public class UpdateUserDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        public bool IsActive { get; set; }

        // Set by system
        public Guid CompanyId { get; set; }
        public Guid UpdatedById { get; set; }
    }

    public class UpdateProfileDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(50)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        // Set by system
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
    }

    public class UpdateUserRoleDto
    {
        public Guid UserId { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;

        public string? Notes { get; set; }

        // Set by system
        public Guid CompanyId { get; set; }
        public Guid UpdatedById { get; set; }
    }

    public class UpdateUserPermissionsDto
    {
        public Guid UserId { get; set; }

        public List<string> Permissions { get; set; } = new();

        // Set by system
        public Guid CompanyId { get; set; }
        public Guid UpdatedById { get; set; }
    }
}