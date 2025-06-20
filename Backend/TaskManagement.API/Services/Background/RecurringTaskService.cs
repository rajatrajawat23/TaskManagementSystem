using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.API.Services.Background
{
    public class RecurringTaskService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RecurringTaskService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Check every hour

        public RecurringTaskService(
            IServiceProvider serviceProvider,
            ILogger<RecurringTaskService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Recurring Task Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessRecurringTasks();
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Recurring Task Service");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Wait 5 minutes before retry
                }
            }

            _logger.LogInformation("Recurring Task Service stopped");
        }

        private async Task ProcessRecurringTasks()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();

            try
            {
                var now = DateTime.UtcNow;
                
                // Find all recurring tasks that need to be processed
                var recurringTasks = await dbContext.Tasks
                    .Where(t => t.IsRecurring && 
                               !string.IsNullOrEmpty(t.RecurrencePattern) &&
                               t.Status != "Cancelled" &&
                               !t.IsArchived)
                    .ToListAsync();

                foreach (var task in recurringTasks)
                {
                    try
                    {
                        var pattern = JsonSerializer.Deserialize<RecurrencePattern>(task.RecurrencePattern!);
                        if (pattern != null && ShouldCreateNewInstance(task, pattern, now))
                        {
                            await CreateRecurringTaskInstance(dbContext, task, pattern, now);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing recurring task {task.TaskNumber}");
                    }
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing recurring tasks");
            }
        }

        private bool ShouldCreateNewInstance(Core.Entities.Task task, RecurrencePattern pattern, DateTime now)
        {
            // Get the last created instance
            var lastInstance = GetLastCreatedInstance(task);
            if (lastInstance == null)
            {
                // No instances created yet, check if we should create the first one
                return task.StartDate == null || task.StartDate <= now;
            }

            var nextDueDate = CalculateNextDueDate(lastInstance, pattern);
            return nextDueDate <= now;
        }

        private DateTime? GetLastCreatedInstance(Core.Entities.Task task)
        {
            // In a real implementation, you might track this in a separate table
            // For now, we'll use the task's last updated date as a simple approach
            return task.UpdatedAt;
        }

        private DateTime CalculateNextDueDate(DateTime? lastInstance, RecurrencePattern pattern)
        {
            if (lastInstance == null)
                return DateTime.UtcNow;

            var lastDate = lastInstance.Value;
            
            return pattern.Type switch
            {
                "Daily" => lastDate.AddDays(pattern.Interval),
                "Weekly" => lastDate.AddDays(7 * pattern.Interval),
                "Monthly" => lastDate.AddMonths(pattern.Interval),
                "Yearly" => lastDate.AddYears(pattern.Interval),
                _ => lastDate.AddDays(1)
            };
        }

        private async Task CreateRecurringTaskInstance(
            TaskManagementDbContext dbContext, 
            Core.Entities.Task originalTask, 
            RecurrencePattern pattern,
            DateTime now)
        {
            var newTask = new Core.Entities.Task
            {
                Title = originalTask.Title,
                Description = originalTask.Description,
                CompanyId = originalTask.CompanyId,
                AssignedToId = originalTask.AssignedToId,
                AssignedById = originalTask.AssignedById,
                ClientId = originalTask.ClientId,
                ProjectId = originalTask.ProjectId,
                Priority = originalTask.Priority,
                Status = "Pending",
                Category = originalTask.Category,
                Tags = originalTask.Tags,
                EstimatedHours = originalTask.EstimatedHours,
                StartDate = now,
                DueDate = CalculateDueDateForNewInstance(now, originalTask, pattern),
                IsRecurring = false, // The instance itself is not recurring
                ParentTaskId = originalTask.Id,
                CreatedById = originalTask.CreatedById,
                UpdatedById = originalTask.UpdatedById,
                CreatedAt = now,
                UpdatedAt = now
            };

            // Generate task number
            newTask.TaskNumber = await GenerateTaskNumber(dbContext, newTask.CompanyId);

            dbContext.Tasks.Add(newTask);
            
            // Update the original task's last processed time
            originalTask.UpdatedAt = now;
            
            _logger.LogInformation($"Created recurring task instance: {newTask.TaskNumber} from {originalTask.TaskNumber}");
        }

        private DateTime? CalculateDueDateForNewInstance(DateTime startDate, Core.Entities.Task originalTask, RecurrencePattern pattern)
        {
            if (originalTask.DueDate == null || originalTask.StartDate == null)
                return null;

            var originalDuration = (originalTask.DueDate.Value - originalTask.StartDate.Value).TotalDays;
            return startDate.AddDays(originalDuration);
        }

        private async Task<string> GenerateTaskNumber(TaskManagementDbContext dbContext, Guid companyId)
        {
            var year = DateTime.UtcNow.Year;
            var lastTask = await dbContext.Tasks
                .Where(t => t.CompanyId == companyId && t.TaskNumber != null && t.TaskNumber.StartsWith($"TSK-{year}-"))
                .OrderByDescending(t => t.TaskNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastTask != null && !string.IsNullOrEmpty(lastTask.TaskNumber))
            {
                var parts = lastTask.TaskNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out var lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"TSK-{year}-{nextNumber:D4}";
        }
    }

    public class RecurrencePattern
    {
        public string Type { get; set; } = "Daily"; // Daily, Weekly, Monthly, Yearly
        public int Interval { get; set; } = 1; // Every N days/weeks/months/years
        public List<int>? DaysOfWeek { get; set; } // For weekly: 0=Sunday, 1=Monday, etc.
        public int? DayOfMonth { get; set; } // For monthly: specific day of month
        public DateTime? EndDate { get; set; } // When to stop creating recurring tasks
        public int? MaxOccurrences { get; set; } // Maximum number of occurrences
    }
}
