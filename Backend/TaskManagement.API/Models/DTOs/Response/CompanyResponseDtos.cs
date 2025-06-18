namespace TaskManagement.API.Models.DTOs.Response
{
    public class CompanyResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public string? Address { get; set; }
        public string SubscriptionType { get; set; } = string.Empty;
        public DateTime? SubscriptionExpiryDate { get; set; }
        public int MaxUsers { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ActiveUsersCount { get; set; }
        public int TotalUsersCount { get; set; }
        public int ProjectsCount { get; set; }
        public int TasksCount { get; set; }
        public int UserCount { get; set; }
    }

    public class CompanyStatisticsDto
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        
        // Users
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int MaxUsersAllowed { get; set; }
        public Dictionary<string, int> UsersByRole { get; set; } = new();
        
        // Projects
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public Dictionary<string, int> ProjectsByStatus { get; set; } = new();
        
        // Tasks
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public Dictionary<string, int> TasksByStatus { get; set; } = new();
        public Dictionary<string, int> TasksByPriority { get; set; } = new();
        
        // Clients
        public int TotalClients { get; set; }
        public int ActiveClients { get; set; }
        
        // Financial
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal PendingPayments { get; set; }
        
        // Activity
        public DateTime? LastActivityDate { get; set; }
        public int ActiveTasksThisMonth { get; set; }
        public int CompletedTasksThisMonth { get; set; }
        public double AverageTaskCompletionTime { get; set; } // In hours
        public double TaskCompletionRate { get; set; } // Percentage
        
        // Subscription
        public string SubscriptionType { get; set; } = string.Empty;
        public DateTime? SubscriptionExpiryDate { get; set; }
        public int DaysUntilExpiry { get; set; }
        public bool IsSubscriptionActive { get; set; }
    }
}