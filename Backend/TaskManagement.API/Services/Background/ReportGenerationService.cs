using Microsoft.EntityFrameworkCore;
using System.Text;
using TaskManagement.Infrastructure.Data;
using TaskManagement.API.Services.Interfaces;

namespace TaskManagement.API.Services.Background
{
    public class ReportGenerationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReportGenerationService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromDays(7); // Weekly reports

        public ReportGenerationService(
            IServiceProvider serviceProvider,
            ILogger<ReportGenerationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Report Generation Service started");

            // Wait for initial delay
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await GenerateWeeklyReports();
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Report Generation Service");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Wait 1 hour before retry
                }
            }

            _logger.LogInformation("Report Generation Service stopped");
        }

        private async Task GenerateWeeklyReports()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            try
            {
                _logger.LogInformation("Starting weekly report generation");

                // Get all active companies
                var companies = await dbContext.Companies
                    .Where(c => c.IsActive && !c.IsDeleted)
                    .ToListAsync();

                foreach (var company in companies)
                {
                    try
                    {
                        await GenerateCompanyWeeklyReport(dbContext, emailService, company);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error generating report for company {company.Name}");
                    }
                }

                _logger.LogInformation("Weekly report generation completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during report generation");
            }
        }

        private async Task GenerateCompanyWeeklyReport(
            TaskManagementDbContext dbContext, 
            IEmailService emailService,
            Core.Entities.Company company)
        {
            var endDate = DateTime.UtcNow.Date;
            var startDate = endDate.AddDays(-7);

            // Get company admins
            var companyAdmins = await dbContext.Users
                .Where(u => u.CompanyId == company.Id && 
                           u.Role == "CompanyAdmin" && 
                           u.IsActive && 
                           !u.IsDeleted)
                .ToListAsync();

            if (!companyAdmins.Any())
                return;

            // Generate report data
            var reportData = await GenerateReportData(dbContext, company.Id, startDate, endDate);
            var reportHtml = GenerateReportHtml(company.Name, startDate, endDate, reportData);

            // Send report to all company admins
            foreach (var admin in companyAdmins)
            {
                try
                {
                    await emailService.SendEmailAsync(
                        admin.Email,
                        $"Weekly Report - {company.Name} ({startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd})",
                        reportHtml);
                    
                    _logger.LogInformation($"Weekly report sent to {admin.Email} for company {company.Name}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send report to {admin.Email}");
                }
            }
        }

        private async Task<WeeklyReportData> GenerateReportData(
            TaskManagementDbContext dbContext, 
            Guid companyId, 
            DateTime startDate, 
            DateTime endDate)
        {
            var reportData = new WeeklyReportData();

            // Tasks created this week
            reportData.TasksCreated = await dbContext.Tasks
                .Where(t => t.CompanyId == companyId && 
                           t.CreatedAt >= startDate && 
                           t.CreatedAt < endDate.AddDays(1))
                .CountAsync();

            // Tasks completed this week
            reportData.TasksCompleted = await dbContext.Tasks
                .Where(t => t.CompanyId == companyId && 
                           t.CompletedDate >= startDate && 
                           t.CompletedDate < endDate.AddDays(1))
                .CountAsync();

            // Overdue tasks
            reportData.OverdueTasks = await dbContext.Tasks
                .Where(t => t.CompanyId == companyId && 
                           t.DueDate < endDate && 
                           t.Status != "Completed" && 
                           t.Status != "Cancelled" &&
                           !t.IsArchived)
                .CountAsync();

            // Active projects
            reportData.ActiveProjects = await dbContext.Projects
                .Where(p => p.CompanyId == companyId && 
                           p.Status == "Active" && 
                           !p.IsDeleted)
                .CountAsync();

            // User activity
            reportData.ActiveUsers = await dbContext.Users
                .Where(u => u.CompanyId == companyId && 
                           u.LastLoginAt >= startDate && 
                           u.LastLoginAt < endDate.AddDays(1))
                .CountAsync();

            // Top performers (users who completed most tasks)
            reportData.TopPerformers = await dbContext.Tasks
                .Where(t => t.CompanyId == companyId && 
                           t.CompletedDate >= startDate && 
                           t.CompletedDate < endDate.AddDays(1) &&
                           t.AssignedToId != null)
                .Include(t => t.AssignedTo)
                .GroupBy(t => new { t.AssignedToId, t.AssignedTo.FirstName, t.AssignedTo.LastName })
                .Select(g => new UserPerformance
                {
                    UserId = g.Key.AssignedToId!.Value,
                    UserName = $"{g.Key.FirstName} {g.Key.LastName}",
                    TasksCompleted = g.Count()
                })
                .OrderByDescending(p => p.TasksCompleted)
                .Take(5)
                .ToListAsync();

            // Upcoming deadlines (next week)
            reportData.UpcomingDeadlines = await dbContext.Tasks
                .Where(t => t.CompanyId == companyId && 
                           t.DueDate >= endDate && 
                           t.DueDate < endDate.AddDays(7) &&
                           t.Status != "Completed" && 
                           t.Status != "Cancelled" &&
                           !t.IsArchived)
                .Include(t => t.AssignedTo)
                .Select(t => new UpcomingTask
                {
                    TaskNumber = t.TaskNumber ?? "N/A",
                    Title = t.Title,
                    DueDate = t.DueDate!.Value,
                    AssignedTo = t.AssignedTo != null ? 
                        $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : "Unassigned",
                    Priority = t.Priority ?? "Medium"
                })
                .OrderBy(t => t.DueDate)
                .Take(10)
                .ToListAsync();

            return reportData;
        }

        private string GenerateReportHtml(string companyName, DateTime startDate, DateTime endDate, WeeklyReportData data)
        {
            var html = new StringBuilder();
            
            html.AppendLine(@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; color: #333; }
        .header { background-color: #f8f9fa; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .metric { background-color: #e9ecef; padding: 15px; margin: 10px 0; border-radius: 5px; }
        .metric-value { font-size: 24px; font-weight: bold; color: #007bff; }
        table { width: 100%; border-collapse: collapse; margin: 20px 0; }
        th, td { padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }
        th { background-color: #f8f9fa; font-weight: bold; }
        .priority-high { color: #dc3545; }
        .priority-medium { color: #ffc107; }
        .priority-low { color: #28a745; }
    </style>
</head>
<body>
    <div class='header'>
        <h1>Weekly Report - " + companyName + @"</h1>
        <p>" + startDate.ToString("MMMM d, yyyy") + " - " + endDate.ToString("MMMM d, yyyy") + @"</p>
    </div>
    
    <div class='content'>
        <h2>Summary</h2>
        
        <div class='metric'>
            <h3>Tasks Created</h3>
            <div class='metric-value'>" + data.TasksCreated + @"</div>
        </div>
        
        <div class='metric'>
            <h3>Tasks Completed</h3>
            <div class='metric-value'>" + data.TasksCompleted + @"</div>
        </div>
        
        <div class='metric'>
            <h3>Overdue Tasks</h3>
            <div class='metric-value'>" + data.OverdueTasks + @"</div>
        </div>
        
        <div class='metric'>
            <h3>Active Projects</h3>
            <div class='metric-value'>" + data.ActiveProjects + @"</div>
        </div>
        
        <div class='metric'>
            <h3>Active Users</h3>
            <div class='metric-value'>" + data.ActiveUsers + @"</div>
        </div>");

            // Top performers
            if (data.TopPerformers.Any())
            {
                html.AppendLine(@"
        <h2>Top Performers</h2>
        <table>
            <tr>
                <th>User</th>
                <th>Tasks Completed</th>
            </tr>");

                foreach (var performer in data.TopPerformers)
                {
                    html.AppendLine($@"
            <tr>
                <td>{performer.UserName}</td>
                <td>{performer.TasksCompleted}</td>
            </tr>");
                }

                html.AppendLine("</table>");
            }

            // Upcoming deadlines
            if (data.UpcomingDeadlines.Any())
            {
                html.AppendLine(@"
        <h2>Upcoming Deadlines (Next 7 Days)</h2>
        <table>
            <tr>
                <th>Task #</th>
                <th>Title</th>
                <th>Due Date</th>
                <th>Assigned To</th>
                <th>Priority</th>
            </tr>");

                foreach (var task in data.UpcomingDeadlines)
                {
                    var priorityClass = task.Priority.ToLower() switch
                    {
                        "high" => "priority-high",
                        "low" => "priority-low",
                        _ => "priority-medium"
                    };

                    html.AppendLine($@"
            <tr>
                <td>{task.TaskNumber}</td>
                <td>{task.Title}</td>
                <td>{task.DueDate:yyyy-MM-dd}</td>
                <td>{task.AssignedTo}</td>
                <td class='{priorityClass}'>{task.Priority}</td>
            </tr>");
                }

                html.AppendLine("</table>");
            }

            html.AppendLine(@"
    </div>
</body>
</html>");

            return html.ToString();
        }
    }

    public class WeeklyReportData
    {
        public int TasksCreated { get; set; }
        public int TasksCompleted { get; set; }
        public int OverdueTasks { get; set; }
        public int ActiveProjects { get; set; }
        public int ActiveUsers { get; set; }
        public List<UserPerformance> TopPerformers { get; set; } = new();
        public List<UpcomingTask> UpcomingDeadlines { get; set; } = new();
    }

    public class UserPerformance
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TasksCompleted { get; set; }
    }

    public class UpcomingTask
    {
        public string TaskNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string AssignedTo { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
    }
}
