using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.DTOs.Request
{
    public class CreateTaskDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(5000)]
        public string? Description { get; set; }

        public Guid? AssignedToId { get; set; }
        
        public Guid? ClientId { get; set; }
        
        public Guid? ProjectId { get; set; }

        [Required]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical

        public string Status { get; set; } = "Pending"; // Pending, InProgress, Review, Completed, Cancelled

        public string? Category { get; set; }

        public List<string>? Tags { get; set; }

        public decimal? EstimatedHours { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsRecurring { get; set; }

        public string? RecurrencePattern { get; set; }

        public Guid? ParentTaskId { get; set; }

        // Set by system
        public Guid CompanyId { get; set; }
        public Guid AssignedById { get; set; }
    }

    public class UpdateTaskDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(5000)]
        public string? Description { get; set; }

        public Guid? AssignedToId { get; set; }

        public Guid? ClientId { get; set; }

        public Guid? ProjectId { get; set; }

        [Required]
        public string Priority { get; set; } = "Medium";

        public string Status { get; set; } = "Pending";

        public string? Category { get; set; }

        public List<string>? Tags { get; set; }

        public decimal? EstimatedHours { get; set; }

        public decimal? ActualHours { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsRecurring { get; set; }

        public string? RecurrencePattern { get; set; }

        public int Progress { get; set; }

        // Set by system
        public Guid CompanyId { get; set; }
        public Guid UpdatedById { get; set; }
    }

    public class AssignTaskDto
    {
        public Guid TaskId { get; set; }

        [Required]
        public Guid AssignedToId { get; set; }

        public string? Notes { get; set; }

        // Set by system
        public Guid CompanyId { get; set; }
        public Guid AssignedById { get; set; }
    }

    public class UpdateTaskStatusDto
    {
        public Guid TaskId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public string? Notes { get; set; }

        // Set by system
        public Guid CompanyId { get; set; }
        public Guid UpdatedById { get; set; }
    }

    public class CreateTaskCommentDto
    {
        public Guid TaskId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;

        // Set by system
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
    }
}