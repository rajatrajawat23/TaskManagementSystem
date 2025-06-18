using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Core.Entities
{
    public class UserPermission : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string PermissionType { get; set; }

        [Required]
        [MaxLength(50)]
        public string PermissionValue { get; set; }

        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

        public Guid? GrantedById { get; set; }

        public DateTime? ExpiryDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual User GrantedBy { get; set; }
    }
}