namespace TaskManagement.API.Models.DTOs.Response
{
    public class ClientResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Website { get; set; }
        public string? Industry { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ClientContactResponseDto> Contacts { get; set; } = new();
        public int ProjectCount { get; set; }
        public int ActiveProjectCount { get; set; }
        public int TaskCount { get; set; }
    }

    public class ClientContactResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class ClientStatisticsDto
    {
        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal PendingPayments { get; set; }
        public double AverageProjectDuration { get; set; } // In days
        public double ClientSatisfactionScore { get; set; } // 0-5
        public List<ProjectSummaryDto> RecentProjects { get; set; } = new();
        public Dictionary<string, decimal> RevenueByMonth { get; set; } = new();
    }

    public class ProjectSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Budget { get; set; }
        public int Progress { get; set; }
    }
}