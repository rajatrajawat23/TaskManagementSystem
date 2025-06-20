using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Services.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.API.Services.Background
{
    public class EmailNotificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailNotificationService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30); // Check every 30 minutes

        public EmailNotificationService(
            IServiceProvider serviceProvider,
            ILogger<EmailNotificationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email Notification Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessTaskReminders();
                    await ProcessOverdueTasks();
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Email Notification Service");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Wait 5 minutes before retry
                }
            }

            _logger.LogInformation("Email Notification Service stopped");
        }

        private async Task ProcessTaskReminders()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            try
            {
                var tomorrow = DateTime.UtcNow.AddDays(1).Date;
                var dayAfterTomorrow = tomorrow.AddDays(1);

                // Find tasks due tomorrow
                var tasksDueTomorrow = await dbContext.Tasks
                    .Include(t => t.AssignedTo)
                    .Where(t => t.DueDate >= tomorrow && 
                               t.DueDate < dayAfterTomorrow &&
                               t.Status != "Completed" &&
                               t.Status != "Cancelled" &&
                               !t.IsArchived &&
                               t.AssignedTo != null &&
                               t.AssignedTo.IsActive)
                    .ToListAsync();

                foreach (var task in tasksDueTomorrow)
                {
                    if (task.AssignedTo != null && !string.IsNullOrEmpty(task.AssignedTo.Email))
                    {
                        try
                        {
                            // Use integrated notification service
                            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                            await notificationService.SendTaskReminderNotificationAsync(task.AssignedTo.Id, task);
                            
                            _logger.LogInformation($"Reminder notification sent for task {task.TaskNumber} to user {task.AssignedTo.Id}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to send reminder notification for task {task.TaskNumber}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing task reminders");
            }
        }

        private async Task ProcessOverdueTasks()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            try
            {
                var now = DateTime.UtcNow;
                var oneDayAgo = now.AddDays(-1);

                // Find tasks that just became overdue (within last 24 hours)
                var newlyOverdueTasks = await dbContext.Tasks
                    .Include(t => t.AssignedTo)
                    .Include(t => t.AssignedBy)
                    .Where(t => t.DueDate < now && 
                               t.DueDate >= oneDayAgo &&
                               t.Status != "Completed" &&
                               t.Status != "Cancelled" &&
                               !t.IsArchived)
                    .ToListAsync();

                foreach (var task in newlyOverdueTasks)
                {
                    // Notify assignee
                    if (task.AssignedTo != null && !string.IsNullOrEmpty(task.AssignedTo.Email))
                    {
                        try
                        {
                            // Use integrated notification service
                            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                            await notificationService.SendTaskOverdueNotificationAsync(task.AssignedTo.Id, task);
                            
                            _logger.LogInformation($"Overdue notification sent for task {task.TaskNumber} to assignee");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to send overdue notification for task {task.TaskNumber}");
                        }
                    }

                    // Also notify the person who assigned the task
                    if (task.AssignedBy != null && 
                        !string.IsNullOrEmpty(task.AssignedBy.Email) &&
                        task.AssignedBy.Id != task.AssignedTo?.Id)
                    {
                        try
                        {
                            await emailService.SendTaskReminderEmailAsync(
                                task.AssignedBy.Email,
                                $"OVERDUE: {task.Title} (Assigned to {task.AssignedTo?.FirstName} {task.AssignedTo?.LastName})",
                                task.DueDate!.Value);
                            
                            _logger.LogInformation($"Overdue notification sent for task {task.TaskNumber} to assigner");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to send overdue notification to assigner for task {task.TaskNumber}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing overdue tasks");
            }
        }
    }
}
