using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.API.Services.Interfaces;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Services.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(IUnitOfWork unitOfWork, ILogger<DashboardService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DashboardDto> GetDashboardAsync(Guid? companyId, Guid? userId, string? userRole, 
            DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var dashboard = new DashboardDto();
                
                // Get task summary
                var tasksQuery = _unitOfWork.Tasks.Query().Where(t => !t.IsDeleted);
                
                if (companyId.HasValue)
                    tasksQuery = tasksQuery.Where(t => t.CompanyId == companyId.Value);
                
                if (userId.HasValue && userRole != "Manager" && userRole != "CompanyAdmin" && userRole != "SuperAdmin")
                    tasksQuery = tasksQuery.Where(t => t.AssignedToId == userId.Value);

                if (startDate.HasValue)
                    tasksQuery = tasksQuery.Where(t => t.CreatedAt >= startDate.Value);
                
                if (endDate.HasValue)
                    tasksQuery = tasksQuery.Where(t => t.CreatedAt <= endDate.Value);

                var tasks = await tasksQuery.ToListAsync();
                
                dashboard.TaskSummary = new TaskSummaryDto
                {
                    TotalTasks = tasks.Count,
                    PendingTasks = tasks.Count(t => t.Status == "Pending"),
                    InProgressTasks = tasks.Count(t => t.Status == "InProgress"),
                    CompletedTasks = tasks.Count(t => t.Status == "Completed"),
                    OverdueTasks = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != "Completed"),
                    DueToday = tasks.Count(t => t.DueDate?.Date == DateTime.UtcNow.Date),
                    DueThisWeek = tasks.Count(t => t.DueDate >= DateTime.UtcNow && t.DueDate <= DateTime.UtcNow.AddDays(7))
                };

                // Get project summary
                var projectsQuery = _unitOfWork.Projects.Query().Where(p => !p.IsDeleted);
                
                if (companyId.HasValue)
                    projectsQuery = projectsQuery.Where(p => p.CompanyId == companyId.Value);
                
                var projects = await projectsQuery.ToListAsync();
                
                dashboard.ProjectSummary = new DashboardProjectSummaryDto
                {
                    TotalProjects = projects.Count,
                    ActiveProjects = projects.Count(p => p.Status == "Active"),
                    CompletedProjects = projects.Count(p => p.Status == "Completed"),
                    OnHoldProjects = projects.Count(p => p.Status == "On Hold")
                };

                // Get upcoming tasks
                var upcomingTasksQuery = tasksQuery
                    .Where(t => t.DueDate != null && t.DueDate >= DateTime.UtcNow && t.Status != "Completed")
                    .OrderBy(t => t.DueDate)
                    .Take(10);

                dashboard.UpcomingTasks = await upcomingTasksQuery
                    .Select(t => new UpcomingTaskDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        DueDate = t.DueDate,
                        Priority = t.Priority ?? "Medium",
                        Status = t.Status ?? "Pending",
                        AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : null
                    })
                    .ToListAsync();

                // Task by category
                dashboard.TasksByCategory = tasks
                    .Where(t => !string.IsNullOrEmpty(t.Category))
                    .GroupBy(t => t.Category!)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Performance metrics
                var completedTasks = tasks.Where(t => t.Status == "Completed").ToList();
                
                dashboard.PerformanceMetrics = new PerformanceMetricsDto
                {
                    TaskCompletionRate = tasks.Any() ? (double)completedTasks.Count / tasks.Count * 100 : 0,
                    TasksCompletedThisWeek = completedTasks.Count(t => t.CompletedDate >= DateTime.UtcNow.AddDays(-7)),
                    TasksCompletedThisMonth = completedTasks.Count(t => t.CompletedDate >= DateTime.UtcNow.AddDays(-30)),
                    OnTimeDeliveryRate = completedTasks.Any() ? 
                        (double)completedTasks.Count(t => t.CompletedDate <= t.DueDate) / completedTasks.Count * 100 : 0,
                    AverageTaskCompletionTime = completedTasks.Any() ? 
                        completedTasks.Average(t => (t.CompletedDate!.Value - t.CreatedAt).TotalHours) : 0
                };

                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard data");
                throw;
            }
        }

        public async Task<CompanyOverviewDto> GetCompanyOverviewAsync(Guid companyId)
        {
            try
            {
                var overview = new CompanyOverviewDto();
                
                // Get company statistics
                var statistics = new CompanyStatisticsDto();
                
                var users = await _unitOfWork.Users.Query()
                    .Where(u => u.CompanyId == companyId && !u.IsDeleted)
                    .ToListAsync();
                
                statistics.TotalUsers = users.Count;
                statistics.ActiveUsers = users.Count(u => u.IsActive);
                statistics.UsersByRole = users.GroupBy(u => u.Role).ToDictionary(g => g.Key, g => g.Count());
                
                var projects = await _unitOfWork.Projects.Query()
                    .Where(p => p.CompanyId == companyId && !p.IsDeleted)
                    .ToListAsync();
                
                statistics.TotalProjects = projects.Count;
                statistics.ActiveProjects = projects.Count(p => p.Status == "Active");
                statistics.ProjectsByStatus = projects.GroupBy(p => p.Status ?? "Unknown")
                    .ToDictionary(g => g.Key, g => g.Count());
                
                var clients = await _unitOfWork.Clients.Query()
                    .Where(c => c.CompanyId == companyId && !c.IsDeleted)
                    .ToListAsync();
                
                statistics.TotalClients = clients.Count;
                
                var tasks = await _unitOfWork.Tasks.Query()
                    .Where(t => t.CompanyId == companyId && !t.IsDeleted)
                    .ToListAsync();
                
                statistics.TotalTasks = tasks.Count;
                statistics.CompletedTasks = tasks.Count(t => t.Status == "Completed");
                statistics.OverdueTasks = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != "Completed");
                statistics.TaskCompletionRate = tasks.Any() ? (double)statistics.CompletedTasks / statistics.TotalTasks * 100 : 0;
                statistics.TasksByStatus = tasks.GroupBy(t => t.Status ?? "Unknown")
                    .ToDictionary(g => g.Key, g => g.Count());
                
                overview.Statistics = statistics;
                
                // Get top performers
                var userTaskStats = await _unitOfWork.Tasks.Query()
                    .Where(t => t.CompanyId == companyId && t.AssignedToId != null && !t.IsDeleted)
                    .Include(t => t.AssignedTo)
                    .GroupBy(t => new { t.AssignedToId, t.AssignedTo!.FirstName, t.AssignedTo.LastName })
                    .Select(g => new UserActivityDto
                    {
                        UserId = g.Key.AssignedToId!.Value,
                        UserName = $"{g.Key.FirstName} {g.Key.LastName}",
                        TasksCompleted = g.Count(t => t.Status == "Completed"),
                        OnTimeDeliveryRate = g.Any(t => t.Status == "Completed") ? 
                            (double)g.Count(t => t.Status == "Completed" && t.CompletedDate <= t.DueDate) / 
                            g.Count(t => t.Status == "Completed") * 100 : 0
                    })
                    .OrderByDescending(u => u.TasksCompleted)
                    .Take(5)
                    .ToListAsync();
                
                overview.TopPerformers = userTaskStats;
                
                // Get active projects summary
                var activeProjects = await _unitOfWork.Projects.Query()
                    .Where(p => p.CompanyId == companyId && p.Status == "Active" && !p.IsDeleted)
                    .Include(p => p.Tasks)
                    .Select(p => new ProjectSummaryItemDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Status = p.Status ?? "Unknown",
                        DueDate = p.EndDate,
                        TaskCount = p.Tasks.Count(t => !t.IsDeleted),
                        CompletedTaskCount = p.Tasks.Count(t => t.Status == "Completed" && !t.IsDeleted),
                        CompletionPercentage = p.Tasks.Any(t => !t.IsDeleted) ? 
                            (double)p.Tasks.Count(t => t.Status == "Completed" && !t.IsDeleted) / 
                            p.Tasks.Count(t => !t.IsDeleted) * 100 : 0
                    })
                    .OrderBy(p => p.DueDate)
                    .Take(10)
                    .ToListAsync();
                
                overview.ActiveProjects = activeProjects;
                
                return overview;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company overview");
                throw;
            }
        }

        public async Task<SystemOverviewDto> GetSystemOverviewAsync()
        {
            try
            {
                var overview = new SystemOverviewDto();
                
                var companies = await _unitOfWork.Companies.Query()
                    .Where(c => !c.IsDeleted)
                    .ToListAsync();
                
                overview.TotalCompanies = companies.Count;
                overview.ActiveCompanies = companies.Count(c => c.IsActive);
                overview.CompaniesBySubscription = companies
                    .GroupBy(c => c.SubscriptionType ?? "Free")
                    .ToDictionary(g => g.Key, g => g.Count());
                
                var users = await _unitOfWork.Users.Query()
                    .Where(u => !u.IsDeleted)
                    .ToListAsync();
                
                overview.TotalUsers = users.Count;
                overview.ActiveUsers = users.Count(u => u.IsActive);
                
                var tasks = await _unitOfWork.Tasks.Query()
                    .Where(t => !t.IsDeleted)
                    .ToListAsync();
                
                overview.TotalTasks = tasks.Count;
                overview.TasksCreatedToday = tasks.Count(t => t.CreatedAt.Date == DateTime.UtcNow.Date);
                
                // Get top companies by activity
                var topCompanies = await _unitOfWork.Companies.Query()
                    .Where(c => !c.IsDeleted && c.IsActive)
                    .Include(c => c.Users)
                    .Include(c => c.Tasks)
                    .Include(c => c.Projects)
                    .Select(c => new CompanyActivityDto
                    {
                        CompanyId = c.Id,
                        CompanyName = c.Name,
                        UserCount = c.Users.Count(u => !u.IsDeleted),
                        TaskCount = c.Tasks.Count(t => !t.IsDeleted),
                        ProjectCount = c.Projects.Count(p => !p.IsDeleted),
                        LastActivityDate = c.UpdatedAt
                    })
                    .OrderByDescending(c => c.TaskCount)
                    .Take(10)
                    .ToListAsync();
                
                overview.TopCompanies = topCompanies;
                
                return overview;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting system overview");
                throw;
            }
        }

        public async Task<UserPerformanceDto> GetUserPerformanceAsync(Guid userId, Guid companyId, 
            DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var user = await _unitOfWork.Users.Query()
                    .FirstOrDefaultAsync(u => u.Id == userId && u.CompanyId == companyId);
                
                if (user == null)
                    throw new KeyNotFoundException("User not found");
                
                var performance = new UserPerformanceDto
                {
                    UserId = userId,
                    UserName = $"{user.FirstName} {user.LastName}"
                };
                
                var tasksQuery = _unitOfWork.Tasks.Query()
                    .Where(t => t.AssignedToId == userId && !t.IsDeleted);
                
                if (startDate.HasValue)
                    tasksQuery = tasksQuery.Where(t => t.CreatedAt >= startDate.Value);
                
                if (endDate.HasValue)
                    tasksQuery = tasksQuery.Where(t => t.CreatedAt <= endDate.Value);
                
                var tasks = await tasksQuery.ToListAsync();
                
                performance.TotalTasksAssigned = tasks.Count;
                performance.TasksCompleted = tasks.Count(t => t.Status == "Completed");
                performance.TasksInProgress = tasks.Count(t => t.Status == "InProgress");
                performance.TasksOverdue = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != "Completed");
                performance.CompletionRate = tasks.Any() ? (double)performance.TasksCompleted / tasks.Count * 100 : 0;
                
                var completedTasks = tasks.Where(t => t.Status == "Completed").ToList();
                performance.OnTimeDeliveryRate = completedTasks.Any() ? 
                    (double)completedTasks.Count(t => t.CompletedDate <= t.DueDate) / completedTasks.Count * 100 : 0;
                performance.AverageCompletionTime = completedTasks.Any() ? 
                    completedTasks.Average(t => (t.CompletedDate!.Value - t.CreatedAt).TotalHours) : 0;
                
                performance.TasksByPriority = tasks
                    .GroupBy(t => t.Priority ?? "Medium")
                    .ToDictionary(g => g.Key, g => g.Count());
                
                performance.TasksByCategory = tasks
                    .Where(t => !string.IsNullOrEmpty(t.Category))
                    .GroupBy(t => t.Category!)
                    .ToDictionary(g => g.Key, g => g.Count());
                
                return performance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user performance");
                throw;
            }
        }

        public async Task<IEnumerable<ActivityDto>> GetRecentActivitiesAsync(Guid? companyId, Guid? userId, 
            string? userRole, int count)
        {
            try
            {
                var activities = new List<ActivityDto>();
                
                // Get recent tasks
                IQueryable<Core.Entities.Task> tasksQuery = _unitOfWork.Tasks.Query()
                    .Where(t => !t.IsDeleted)
                    .Include(t => t.AssignedTo)
                    .Include(t => t.AssignedBy);
                
                if (companyId.HasValue)
                    tasksQuery = tasksQuery.Where(t => t.CompanyId == companyId.Value);
                
                if (userId.HasValue && userRole != "Manager" && userRole != "CompanyAdmin" && userRole != "SuperAdmin")
                    tasksQuery = tasksQuery.Where(t => t.AssignedToId == userId.Value || t.AssignedById == userId.Value);
                
                var recentTasks = await tasksQuery
                    .OrderByDescending(t => t.UpdatedAt)
                    .Take(count)
                    .ToListAsync();
                
                foreach (var task in recentTasks)
                {
                    activities.Add(new ActivityDto
                    {
                        Id = Guid.NewGuid(),
                        Type = "TaskUpdate",
                        Description = $"Task '{task.Title}' was updated",
                        Timestamp = task.UpdatedAt,
                        UserId = task.UpdatedById,
                        UserName = task.AssignedBy != null ? $"{task.AssignedBy.FirstName} {task.AssignedBy.LastName}" : "System",
                        EntityType = "Task",
                        EntityId = task.Id,
                        EntityName = task.Title
                    });
                }
                
                return activities.OrderByDescending(a => a.Timestamp).Take(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent activities");
                throw;
            }
        }
    }
}