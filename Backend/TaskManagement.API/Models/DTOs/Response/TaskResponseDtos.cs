namespace TaskManagement.API.Models.DTOs.Response
{
    public class TaskResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TaskNumber { get; set; } = string.Empty;
        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public Guid AssignedById { get; set; }
        public string AssignedByName { get; set; } = string.Empty;
        public Guid? ClientId { get; set; }
        public string? ClientName { get; set; }
        public Guid? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Category { get; set; }
        public List<string> Tags { get; set; } = new();
        public decimal? EstimatedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurrencePattern { get; set; }
        public Guid? ParentTaskId { get; set; }
        public int Progress { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<SubTaskDto> SubTasks { get; set; } = new();
        public List<TaskCommentDto> Comments { get; set; } = new();
        public List<TaskAttachmentDto> Attachments { get; set; } = new();
    }

    public class SubTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TaskCommentDto
    {
        public Guid Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class TaskAttachmentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public Guid UploadedById { get; set; }
        public string UploadedByName { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }

    public class TaskCalendarDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AssignedToName { get; set; }
        public string Color { get; set; } = string.Empty; // Based on priority or status
    }

    public class TaskStatisticsDto
    {
        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public Dictionary<string, int> TasksByPriority { get; set; } = new();
        public Dictionary<string, int> TasksByCategory { get; set; } = new();
        public Dictionary<string, int> TasksByAssignee { get; set; } = new();
        public Dictionary<string, int> TasksByStatus { get; set; } = new();
        public double AverageCompletionTime { get; set; } // In hours
        public double CompletionRate { get; set; } // Percentage
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}