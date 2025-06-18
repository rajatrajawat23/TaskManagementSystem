namespace TaskManagement.API.Models.DTOs.Response
{
    public class DashboardDto
    {
        public TaskSummaryDto TaskSummary { get; set; } = new();
        public DashboardProjectSummaryDto ProjectSummary { get; set; } = new();
        public List<UpcomingTaskDto> UpcomingTasks { get; set; } = new();
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
        public Dictionary<string, int> TasksByCategory { get; set; } = new();
        public List<ChartDataDto> TaskTrend { get; set; } = new();
        public PerformanceMetricsDto PerformanceMetrics { get; set; } = new();
    }

    public class TaskSummaryDto
    {
        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int DueToday { get; set; }
        public int DueThisWeek { get; set; }
    }

    public class DashboardProjectSummaryDto
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int OnHoldProjects { get; set; }
    }

    public class UpcomingTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AssignedToName { get; set; }
    }

    public class RecentActivityDto
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? RelatedEntity { get; set; }
        public Guid? RelatedEntityId { get; set; }
    }

    public class ChartDataDto
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
        public DateTime? Date { get; set; }
    }

    public class PerformanceMetricsDto
    {
        public double AverageTaskCompletionTime { get; set; }
        public double TaskCompletionRate { get; set; }
        public double OnTimeDeliveryRate { get; set; }
        public int TasksCompletedThisWeek { get; set; }
        public int TasksCompletedThisMonth { get; set; }
    }

    public class CompanyOverviewDto
    {
        public CompanyStatisticsDto Statistics { get; set; } = new();
        public List<UserActivityDto> TopPerformers { get; set; } = new();
        public List<ProjectSummaryItemDto> ActiveProjects { get; set; } = new();
        public Dictionary<string, double> ResourceUtilization { get; set; } = new();
        public List<ChartDataDto> MonthlyTaskTrend { get; set; } = new();
    }

    public class UserActivityDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TasksCompleted { get; set; }
        public double AverageCompletionTime { get; set; }
        public double OnTimeDeliveryRate { get; set; }
    }

    public class ProjectSummaryItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double CompletionPercentage { get; set; }
        public DateTime? DueDate { get; set; }
        public int TaskCount { get; set; }
        public int CompletedTaskCount { get; set; }
    }

    public class SystemOverviewDto
    {
        public int TotalCompanies { get; set; }
        public int ActiveCompanies { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalTasks { get; set; }
        public int TasksCreatedToday { get; set; }
        public Dictionary<string, int> CompaniesBySubscription { get; set; } = new();
        public List<CompanyActivityDto> TopCompanies { get; set; } = new();
        public List<ChartDataDto> SystemGrowth { get; set; } = new();
    }

    public class CompanyActivityDto
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public int TaskCount { get; set; }
        public int ProjectCount { get; set; }
        public DateTime LastActivityDate { get; set; }
    }

    public class UserPerformanceDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TotalTasksAssigned { get; set; }
        public int TasksCompleted { get; set; }
        public int TasksInProgress { get; set; }
        public int TasksOverdue { get; set; }
        public double CompletionRate { get; set; }
        public double AverageCompletionTime { get; set; }
        public double OnTimeDeliveryRate { get; set; }
        public List<ChartDataDto> CompletionTrend { get; set; } = new();
        public Dictionary<string, int> TasksByPriority { get; set; } = new();
        public Dictionary<string, int> TasksByCategory { get; set; } = new();
    }

    public class ActivityDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string? EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public string? EntityName { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }
}