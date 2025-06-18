using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Core.Entities
{
    public class SubTask : BaseEntity
    {
        [Required]
        public Guid TaskId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

        public Guid? CompletedById { get; set; }

        public int SortOrder { get; set; } = 0;

        // Navigation properties
        public virtual Task Task { get; set; }
        public virtual User CompletedBy { get; set; }
    }
}