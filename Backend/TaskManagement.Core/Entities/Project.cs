using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Core.Entities
{
    public class Project : BaseEntity
    {
        [Required]
        public Guid CompanyId { get; set; }

        public Guid? ClientId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [MaxLength(20)]
        public string? ProjectCode { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? Budget { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; } = "Active";

        public Guid? ProjectManagerId { get; set; }

        public string? TeamMembers { get; set; } // JSON array of user IDs

        public int Progress { get; set; } = 0;

        public bool IsArchived { get; set; } = false;

        // Navigation properties
        public virtual Company? Company { get; set; }
        public virtual Client? Client { get; set; }
        public virtual User? ProjectManager { get; set; }
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
        public virtual ICollection<ChatGroup> ChatGroups { get; set; } = new List<ChatGroup>();
    }
}