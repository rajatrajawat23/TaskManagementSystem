using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Core.Entities
{
    public class ChatGroup : BaseEntity
    {
        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(20)]
        public string GroupType { get; set; } = "General";

        public Guid? RelatedProjectId { get; set; }

        [Required]
        public string Members { get; set; } // JSON array of user IDs

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Company Company { get; set; }
        public virtual Project RelatedProject { get; set; }
        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}