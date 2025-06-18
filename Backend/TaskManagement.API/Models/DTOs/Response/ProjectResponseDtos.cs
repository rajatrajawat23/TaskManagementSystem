namespace TaskManagement.API.Models.DTOs.Response
{
    public class ProjectResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public Guid? ClientId { get; set; }
        public string? ClientName { get; set; }
        public Guid ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public string ProjectManagerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal? Budget { get; set; }
        public decimal? ActualCost { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int Progress { get; set; }
        public List<string> Tags { get; set; } = new();
        public bool IsArchived { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<TeamMemberDto> TeamMembers { get; set; } = new();
        public int TaskCount { get; set; }
        public int CompletedTaskCount { get; set; }
    }

    public class TeamMemberDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
    }

    public class ProjectStatisticsDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
        public decimal? BudgetUtilization { get; set; }
        public double AverageTaskCompletionTime { get; set; }
        public Dictionary<string, int> TasksByPriority { get; set; } = new();
        public Dictionary<string, decimal> HoursByTeamMember { get; set; } = new();
        public List<ProjectMilestoneDto> Milestones { get; set; } = new();
    }

    public class ProjectMilestoneDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public int CompletionPercentage { get; set; }
    }
}