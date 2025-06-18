using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Core.Entities
{
    public class User : BaseEntity
    {
        public Guid? CompanyId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(500)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(500)]
        public string PasswordSalt { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? ProfileImageUrl { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; }

        [MaxLength(50)]
        public string? Department { get; set; }

        [MaxLength(100)]
        public string? JobTitle { get; set; }

        public bool IsActive { get; set; } = true;

        public bool EmailVerified { get; set; } = false;

        public DateTime? LastLoginAt { get; set; }

        [MaxLength(500)]
        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetExpiry { get; set; }

        [MaxLength(500)]
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiry { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; }
        public virtual ICollection<Task> AssignedTasks { get; set; } = new List<Task>();
        public virtual ICollection<Task> CreatedTasks { get; set; } = new List<Task>();
        public virtual ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<UserPermission> Permissions { get; set; } = new List<UserPermission>();
    }
}