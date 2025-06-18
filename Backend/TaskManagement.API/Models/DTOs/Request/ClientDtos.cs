using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.DTOs.Request
{
    public class CreateClientDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        public string? Website { get; set; }

        [StringLength(100)]
        public string? Industry { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public string Status { get; set; } = "Active"; // Active, Inactive, Prospect

        public List<ClientContactDto>? Contacts { get; set; }

        // Set by system
        public Guid CompanyId { get; set; }
        public Guid CreatedById { get; set; }
    }

    public class UpdateClientDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        public string? Website { get; set; }

        [StringLength(100)]
        public string? Industry { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public string Status { get; set; } = "Active";

        // Set by system
        public Guid CompanyId { get; set; }
        public Guid UpdatedById { get; set; }
    }

    public class ClientContactDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Title { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        public bool IsPrimary { get; set; }
    }

    public class AddClientContactDto
    {
        public Guid ClientId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Title { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        public bool IsPrimary { get; set; }

        // Set by system
        public Guid CompanyId { get; set; }
    }

    public class UpdateClientStatusDto
    {
        public Guid ClientId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public string? Notes { get; set; }

        // Set by system
        public Guid CompanyId { get; set; }
        public Guid UpdatedById { get; set; }
    }
}