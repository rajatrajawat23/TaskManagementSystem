using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.DTOs.Request
{
    public class CreateCompanyDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Domain { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;

        [Phone]
        public string? ContactPhone { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [Required]
        public string SubscriptionType { get; set; } = "Free"; // Free, Premium, Enterprise

        public DateTime? SubscriptionExpiryDate { get; set; }

        public int MaxUsers { get; set; } = 10;

        // Admin user details
        [Required]
        [EmailAddress]
        public string AdminEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string AdminPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AdminFirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AdminLastName { get; set; } = string.Empty;

        // Set by system
        public Guid CreatedById { get; set; }
    }

    public class UpdateCompanyDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;

        [Phone]
        public string? ContactPhone { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        public int MaxUsers { get; set; }

        // Set by system
        public Guid UpdatedById { get; set; }
    }

    public class UpdateSubscriptionDto
    {
        public Guid CompanyId { get; set; }

        [Required]
        public string SubscriptionType { get; set; } = string.Empty;

        public DateTime? SubscriptionExpiryDate { get; set; }

        public int MaxUsers { get; set; }

        public string? Notes { get; set; }

        // Set by system
        public Guid UpdatedById { get; set; }
    }
}