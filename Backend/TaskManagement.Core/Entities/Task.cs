using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Core.Entities
{
    public class Task : BaseEntity
    {
        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [MaxLength(20)]
        public string? TaskNumber { get; set; }

        public Guid? AssignedToId { get; set; }

        [Required]
        public Guid AssignedById { get; set; }

        public Guid? ClientId { get; set; }

        public Guid? ProjectId { get; set; }

        [MaxLength(10)]
        public string? Priority { get; set; } = "Medium";

        [MaxLength(20)]
        public string? Status { get; set; } = "Pending";

        [MaxLength(50)]
        public string? Category { get; set; }

        [MaxLength(500)]
        public string? Tags { get; set; } // JSON array of tags

        public decimal? EstimatedHours { get; set; }

        public decimal? ActualHours { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public bool IsRecurring { get; set; } = false;

        [MaxLength(100)]
        public string? RecurrencePattern { get; set; } // JSON for recurrence rules

        public Guid? ParentTaskId { get; set; }

        public int Progress { get; set; } = 0;

        public bool IsArchived { get; set; } = false;

        // Navigation properties
        public virtual Company Company { get; set; }
        public virtual User AssignedTo { get; set; }
        public virtual User AssignedBy { get; set; }
        public virtual Client Client { get; set; }
        public virtual Project Project { get; set; }
        public virtual Task ParentTask { get; set; }
        public virtual ICollection<Task> SubTasks { get; set; } = new List<Task>();
        public virtual ICollection<SubTask> SubTaskItems { get; set; } = new List<SubTask>();
        public virtual ICollection<TaskAttachment> Attachments { get; set; } = new List<TaskAttachment>();
        public virtual ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    }
}