using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Core.Entities
{
    public class Client : BaseEntity
    {
        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(200)]
        public string? Website { get; set; }

        [MaxLength(50)]
        public string? Industry { get; set; }

        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Company Company { get; set; }
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}