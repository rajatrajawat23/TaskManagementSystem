using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.API.Services.Interfaces;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace TaskManagement.API.Services.Implementation
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<TaskService> _logger;
        private readonly IFileService _fileService;
        private readonly IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;

        public TaskService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<TaskService> logger,
            IFileService fileService,
            IEmailService emailService,
            IHubContext<NotificationHub> hubContext,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
            _fileService = fileService;
            _emailService = emailService;
            _hubContext = hubContext;
            _notificationService = notificationService;
        }

        public async Task<PagedResult<TaskResponseDto>> GetTasksAsync(
            Guid? companyId, int pageNumber, int pageSize, string? status, string? priority,
            string? assignedToId, DateTime? startDate, DateTime? endDate, string? search,
            string? sortBy, bool sortDescending)
        {
            try
            {
                IQueryable<Core.Entities.Task> query = _unitOfWork.Tasks.Query()
                    .Include(t => t.AssignedTo)
                    .Include(t => t.AssignedBy)
                    .Include(t => t.Client)
                    .Include(t => t.Project);

                // Filter by company only if companyId is provided (SuperAdmin can see all)
                if (companyId.HasValue)
                {
                    query = query.Where(t => t.CompanyId == companyId.Value);
                }

                // Apply filters
                if (!string.IsNullOrEmpty(status))
                    query = query.Where(t => t.Status == status);

                if (!string.IsNullOrEmpty(priority))
                    query = query.Where(t => t.Priority == priority);

                if (!string.IsNullOrEmpty(assignedToId) && Guid.TryParse(assignedToId, out var parsedAssignedToId))
                    query = query.Where(t => t.AssignedToId == parsedAssignedToId);

                if (startDate.HasValue)
                    query = query.Where(t => t.StartDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(t => t.DueDate <= endDate.Value);

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(t => t.Title.Contains(search) || 
                                           (t.Description != null && t.Description.Contains(search)) ||
                                           t.TaskNumber.Contains(search));
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(sortBy))
                {
                    query = sortBy.ToLower() switch
                    {
                        "title" => sortDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                        "priority" => sortDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                        "status" => sortDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
                        "duedate" => sortDescending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
                        _ => sortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
                    };
                }
                else
                {
                    query = query.OrderByDescending(t => t.CreatedAt);
                }

                var totalCount = await query.CountAsync();

                var tasks = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new TaskResponseDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        TaskNumber = t.TaskNumber ?? "",
                        AssignedToId = t.AssignedToId,
                        AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : null,
                        AssignedById = t.AssignedById,
                        AssignedByName = t.AssignedBy != null ? $"{t.AssignedBy.FirstName} {t.AssignedBy.LastName}" : null,
                        ClientId = t.ClientId,
                        ClientName = t.Client != null ? t.Client.Name : null,
                        ProjectId = t.ProjectId,
                        ProjectName = t.Project != null ? t.Project.Name : null,
                        Priority = t.Priority ?? "Medium",
                        Status = t.Status ?? "Pending",
                        Category = t.Category,
                        Tags = string.IsNullOrEmpty(t.Tags) ? new List<string>() : Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(t.Tags) ?? new List<string>(),
                        EstimatedHours = t.EstimatedHours,
                        ActualHours = t.ActualHours,
                        StartDate = t.StartDate,
                        DueDate = t.DueDate,
                        CompletedDate = t.CompletedDate,
                        IsRecurring = t.IsRecurring,
                        RecurrencePattern = t.RecurrencePattern,
                        Progress = t.Progress,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt
                    })
                    .ToListAsync();

                return new PagedResult<TaskResponseDto>
                {
                    Items = tasks,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks");
                throw;
            }
        }

        public async Task<TaskResponseDto?> GetTaskByIdAsync(Guid taskId, Guid? companyId)
        {
            try
            {
                var query = _unitOfWork.Tasks.Query()
                    .Include(t => t.AssignedTo)
                    .Include(t => t.AssignedBy)
                    .Include(t => t.Client)
                    .Include(t => t.Project)
                    .Include(t => t.SubTasks)
                    .Include(t => t.Comments).ThenInclude(c => c.User)
                    .Include(t => t.Attachments).ThenInclude(a => a.UploadedBy)
                    .Where(t => t.Id == taskId);

                // Filter by company only if companyId is provided (SuperAdmin can see all)
                if (companyId.HasValue)
                {
                    query = query.Where(t => t.CompanyId == companyId.Value);
                }

                var task = await query.FirstOrDefaultAsync();

                if (task == null)
                    return null;

                return _mapper.Map<TaskResponseDto>(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task by ID: {TaskId}", taskId);
                throw;
            }
        }

        public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto createTaskRequest)
        {
            try
            {
                var task = _mapper.Map<Core.Entities.Task>(createTaskRequest);
                task.CreatedById = _currentUserService.UserId;
                task.UpdatedById = _currentUserService.UserId;
                task.CompanyId = _currentUserService.CompanyId ?? createTaskRequest.CompanyId;
                task.AssignedById = _currentUserService.UserId ?? createTaskRequest.AssignedById;

                // Generate task number with proper length (max 20 chars)
                var taskCount = await _unitOfWork.Tasks.Query()
                    .Where(t => t.CompanyId == task.CompanyId)
                    .CountAsync();
                task.TaskNumber = $"TSK-{DateTime.UtcNow.Year}-{(taskCount + 1):D4}";

                await _unitOfWork.Tasks.AddAsync(task);
                await _unitOfWork.SaveChangesAsync();

                // For now, return a simple response without complex includes
                var simpleTask = await _unitOfWork.Tasks.Query()
                    .Where(t => t.Id == task.Id)
                    .Select(t => new TaskResponseDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        TaskNumber = t.TaskNumber,
                        Priority = t.Priority,
                        Status = t.Status,
                        Category = t.Category,
                        EstimatedHours = t.EstimatedHours,
                        ActualHours = t.ActualHours,
                        StartDate = t.StartDate,
                        DueDate = t.DueDate,
                        CompletedDate = t.CompletedDate,
                        Progress = t.Progress,
                        IsRecurring = t.IsRecurring,
                        RecurrencePattern = t.RecurrencePattern,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt,
                        Tags = new List<string>()
                    })
                    .FirstOrDefaultAsync();

                return simpleTask ?? new TaskResponseDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                throw;
            }
        }

        public async Task<TaskResponseDto?> UpdateTaskAsync(UpdateTaskDto updateTaskRequest)
        {
            try
            {
                // For SuperAdmin, we don't filter by CompanyId
                var query = _unitOfWork.Tasks.Query().Where(t => t.Id == updateTaskRequest.Id);
                
                // If CompanyId is provided (non-SuperAdmin), filter by it
                if (updateTaskRequest.CompanyId != Guid.Empty)
                {
                    query = query.Where(t => t.CompanyId == updateTaskRequest.CompanyId);
                }
                
                var task = await query.FirstOrDefaultAsync();

                if (task == null)
                    return null;

                // Update the task properties
                _mapper.Map(updateTaskRequest, task);
                task.UpdatedById = updateTaskRequest.UpdatedById;
                task.UpdatedAt = DateTime.UtcNow;

                // Update status-related fields
                if (updateTaskRequest.Status == "Completed" && task.CompletedDate == null)
                {
                    task.CompletedDate = DateTime.UtcNow;
                    task.Progress = 100;
                }
                else if (updateTaskRequest.Status != "Completed")
                {
                    task.CompletedDate = null;
                }

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();

                return await GetTaskByIdAsync(task.Id, task.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task");
                throw;
            }
        }

        public async Task<bool> DeleteTaskAsync(Guid taskId, Guid companyId)
        {
            try
            {
                // For SuperAdmin (companyId == Guid.Empty), don't filter by company
                var query = _unitOfWork.Tasks.Query().Where(t => t.Id == taskId);
                
                if (companyId != Guid.Empty)
                {
                    query = query.Where(t => t.CompanyId == companyId);
                }
                
                var task = await query.FirstOrDefaultAsync();

                if (task == null)
                    return false;

                // Soft delete
                task.IsDeleted = true;
                task.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task");
                throw;
            }
        }

        public async Task<TaskResponseDto?> AssignTaskAsync(AssignTaskDto assignTaskDto)
        {
            try
            {
                var task = await _unitOfWork.Tasks.Query()
                    .FirstOrDefaultAsync(t => t.Id == assignTaskDto.TaskId && t.CompanyId == assignTaskDto.CompanyId);

                if (task == null)
                    return null;

                // Verify the assignee exists and belongs to the same company
                var assignee = await _unitOfWork.Users.Query()
                    .FirstOrDefaultAsync(u => u.Id == assignTaskDto.AssignedToId && u.CompanyId == assignTaskDto.CompanyId);

                if (assignee == null)
                    throw new InvalidOperationException("Assignee not found or does not belong to the same company");

                task.AssignedToId = assignTaskDto.AssignedToId;
                task.AssignedById = assignTaskDto.AssignedById;
                task.UpdatedById = assignTaskDto.AssignedById;
                task.UpdatedAt = DateTime.UtcNow;

                // Add a comment if notes are provided
                if (!string.IsNullOrEmpty(assignTaskDto.Notes))
                {
                    var comment = new TaskComment
                    {
                        TaskId = task.Id,
                        Comment = $"Task assigned to {assignee.FirstName} {assignee.LastName}. {assignTaskDto.Notes}",
                        CreatedById = assignTaskDto.AssignedById,
                        IsInternal = true
                    };

                    await _unitOfWork.TaskComments.AddAsync(comment);
                }

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();

                // Send integrated notification (Email + Real-time + Database)
                try
                {
                    await _notificationService.SendTaskAssignmentNotificationAsync(
                        assignee.Id,
                        task,
                        _currentUserService.UserName ?? "System"
                    );
                    _logger.LogInformation("Integrated notification sent to user {UserId} for task assignment", assignee.Id);
                }
                catch (Exception notifEx)
                {
                    _logger.LogError(notifEx, "Failed to send integrated notification for task {TaskNumber}", task.TaskNumber);
                }

                return await GetTaskByIdAsync(task.Id, task.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task");
                throw;
            }
        }

        public async Task<TaskResponseDto?> UpdateTaskStatusAsync(UpdateTaskStatusDto updateStatusDto)
        {
            try
            {
                // For SuperAdmin, we don't filter by CompanyId
                var query = _unitOfWork.Tasks.Query().Where(t => t.Id == updateStatusDto.TaskId);
                
                // If CompanyId is provided (non-SuperAdmin), filter by it
                if (updateStatusDto.CompanyId != Guid.Empty)
                {
                    query = query.Where(t => t.CompanyId == updateStatusDto.CompanyId);
                }
                
                var task = await query.FirstOrDefaultAsync();

                if (task == null)
                    return null;

                var previousStatus = task.Status;
                task.Status = updateStatusDto.Status;
                task.UpdatedById = updateStatusDto.UpdatedById;
                task.UpdatedAt = DateTime.UtcNow;

                // Handle status-specific logic
                if (updateStatusDto.Status == "Completed")
                {
                    task.CompletedDate = DateTime.UtcNow;
                    task.Progress = 100;
                    if (task.ActualHours == null)
                    {
                        task.ActualHours = CalculateActualHours(task.CreatedAt, DateTime.UtcNow);
                    }
                }
                else if (previousStatus == "Completed" && updateStatusDto.Status != "Completed")
                {
                    task.CompletedDate = null;
                    task.Progress = updateStatusDto.Status == "InProgress" ? 50 : 0;
                }

                // Add a comment if notes are provided
                if (!string.IsNullOrEmpty(updateStatusDto.Notes))
                {
                    var comment = new TaskComment
                    {
                        TaskId = task.Id,
                        Comment = $"Status changed from {previousStatus} to {updateStatusDto.Status}. {updateStatusDto.Notes}",
                        CreatedById = updateStatusDto.UpdatedById,
                        IsInternal = true
                    };

                    await _unitOfWork.TaskComments.AddAsync(comment);
                }

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();

                // Send integrated notification for status update
                try
                {
                    if (task.AssignedToId != null)
                    {
                        await _notificationService.SendTaskStatusUpdateNotificationAsync(
                            task.AssignedToId.Value,
                            task,
                            previousStatus,
                            updateStatusDto.Status
                        );
                        _logger.LogInformation("Task status update notification sent for task {TaskNumber}", task.TaskNumber);
                    }
                }
                catch (Exception notifEx)
                {
                    _logger.LogError(notifEx, "Failed to send task status update notification for task {TaskNumber}", task.TaskNumber);
                }

                return await GetTaskByIdAsync(task.Id, task.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task status");
                throw;
            }
        }

        private decimal CalculateActualHours(DateTime startDate, DateTime endDate)
        {
            var timeSpan = endDate - startDate;
            // Calculate business hours (assuming 8 hours per day, excluding weekends)
            var totalDays = (int)timeSpan.TotalDays;
            var businessDays = 0;

            for (int i = 0; i <= totalDays; i++)
            {
                var currentDay = startDate.AddDays(i);
                if (currentDay.DayOfWeek != DayOfWeek.Saturday && currentDay.DayOfWeek != DayOfWeek.Sunday)
                {
                    businessDays++;
                }
            }

            return businessDays * 8; // Assuming 8 hours per business day
        }

        public async Task<IEnumerable<TaskResponseDto>> GetUserTasksAsync(Guid userId, Guid companyId)
        {
            try
            {
                var tasks = await _unitOfWork.Tasks.Query()
                    .Include(t => t.AssignedTo)
                    .Include(t => t.AssignedBy)
                    .Include(t => t.Client)
                    .Include(t => t.Project)
                    .Where(t => t.CompanyId == companyId && 
                               t.AssignedToId == userId && 
                               !t.IsArchived &&
                               t.Status != "Completed" &&
                               t.Status != "Cancelled")
                    .OrderBy(t => t.DueDate)
                    .ThenBy(t => t.Priority)
                    .ToListAsync();

                return _mapper.Map<List<TaskResponseDto>>(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user tasks");
                throw;
            }
        }

        public async Task<IEnumerable<TaskResponseDto>> GetOverdueTasksAsync(Guid companyId)
        {
            try
            {
                var currentDate = DateTime.UtcNow.Date;
                
                var tasks = await _unitOfWork.Tasks.Query()
                    .Include(t => t.AssignedTo)
                    .Include(t => t.AssignedBy)
                    .Include(t => t.Client)
                    .Include(t => t.Project)
                    .Where(t => t.CompanyId == companyId && 
                               t.DueDate != null &&
                               t.DueDate < currentDate &&
                               t.Status != "Completed" &&
                               t.Status != "Cancelled" &&
                               !t.IsArchived)
                    .OrderBy(t => t.DueDate)
                    .ThenBy(t => t.Priority)
                    .ToListAsync();

                return _mapper.Map<List<TaskResponseDto>>(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting overdue tasks");
                throw;
            }
        }

        public async Task<TaskResponseDto?> DuplicateTaskAsync(Guid taskId, Guid companyId, Guid userId)
        {
            try
            {
                var originalTask = await _unitOfWork.Tasks.Query()
                    .Include(t => t.SubTaskItems)
                    .FirstOrDefaultAsync(t => t.Id == taskId && t.CompanyId == companyId);

                if (originalTask == null)
                    return null;

                // Create a copy of the task
                var duplicatedTask = new Core.Entities.Task
                {
                    CompanyId = originalTask.CompanyId,
                    Title = $"{originalTask.Title} (Copy)",
                    Description = originalTask.Description,
                    AssignedToId = originalTask.AssignedToId,
                    AssignedById = userId,
                    ClientId = originalTask.ClientId,
                    ProjectId = originalTask.ProjectId,
                    Priority = originalTask.Priority,
                    Status = "Pending",
                    Category = originalTask.Category,
                    Tags = originalTask.Tags,
                    EstimatedHours = originalTask.EstimatedHours,
                    StartDate = originalTask.StartDate,
                    DueDate = originalTask.DueDate,
                    IsRecurring = originalTask.IsRecurring,
                    RecurrencePattern = originalTask.RecurrencePattern,
                    ParentTaskId = originalTask.ParentTaskId,
                    Progress = 0,
                    CreatedById = userId,
                    UpdatedById = userId
                };

                // Generate new task number
                var taskCount = await _unitOfWork.Tasks.Query().CountAsync(t => t.CompanyId == companyId);
                duplicatedTask.TaskNumber = $"TSK-{DateTime.UtcNow.Year}-{(taskCount + 1):D4}";

                await _unitOfWork.Tasks.AddAsync(duplicatedTask);

                // Duplicate subtasks if any
                foreach (var subtask in originalTask.SubTaskItems)
                {
                    var duplicatedSubtask = new SubTask
                    {
                        TaskId = duplicatedTask.Id,
                        Title = subtask.Title,
                        Description = subtask.Description,
                        IsCompleted = false,
                        SortOrder = subtask.SortOrder,
                        CreatedById = userId,
                        UpdatedById = userId
                    };

                    await _unitOfWork.SubTasks.AddAsync(duplicatedSubtask);
                }

                await _unitOfWork.SaveChangesAsync();

                return await GetTaskByIdAsync(duplicatedTask.Id, duplicatedTask.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error duplicating task");
                throw;
            }
        }

        public async Task<TaskStatisticsDto> GetTaskStatisticsAsync(Guid? companyId)
        {
            try
            {
                var query = _unitOfWork.Tasks.Query()
                    .Where(t => !t.IsArchived)
                    .Select(t => new 
                    {
                        t.CompanyId,
                        Status = t.Status ?? "Unknown",
                        Priority = t.Priority ?? "None",
                        Category = t.Category ?? "",
                        t.DueDate,
                        t.CompletedDate,
                        t.CreatedAt,
                        t.AssignedToId
                    });
                
                // Filter by company only if companyId is provided (SuperAdmin can see all)
                if (companyId.HasValue)
                {
                    query = query.Where(t => t.CompanyId == companyId.Value);
                }

                var taskData = await query.ToListAsync();

                var statistics = new TaskStatisticsDto
                {
                    TotalTasks = taskData.Count,
                    PendingTasks = taskData.Count(t => t.Status == "Pending"),
                    InProgressTasks = taskData.Count(t => t.Status == "InProgress"),
                    CompletedTasks = taskData.Count(t => t.Status == "Completed"),
                    OverdueTasks = taskData.Count(t => t.DueDate != null && t.DueDate < DateTime.UtcNow && 
                                                  t.Status != "Completed" && t.Status != "Cancelled"),
                    TasksByPriority = taskData.GroupBy(t => t.Priority)
                                           .ToDictionary(g => g.Key, g => g.Count()),
                    TasksByCategory = taskData.Where(t => !string.IsNullOrEmpty(t.Category))
                                           .GroupBy(t => t.Category)
                                           .ToDictionary(g => g.Key, g => g.Count()),
                    TasksByStatus = taskData.GroupBy(t => t.Status)
                                         .ToDictionary(g => g.Key, g => g.Count())
                };

                // Calculate task assignments
                var assignmentQuery = _unitOfWork.Tasks.Query()
                    .Where(t => !t.IsArchived && t.AssignedToId != null);
                
                if (companyId.HasValue)
                {
                    assignmentQuery = assignmentQuery.Where(t => t.CompanyId == companyId.Value);
                }

                var taskAssignments = await assignmentQuery
                    .Include(t => t.AssignedTo)
                    .Select(t => new { 
                        t.AssignedToId, 
                        Name = t.AssignedTo.FirstName + " " + t.AssignedTo.LastName 
                    })
                    .ToListAsync();

                var tasksByAssignee = taskAssignments
                    .GroupBy(t => new { t.AssignedToId, t.Name })
                    .ToDictionary(g => g.Key.Name, g => g.Count());

                statistics.TasksByAssignee = tasksByAssignee;

                // Calculate average completion time
                var completedTasks = taskData.Where(t => t.Status == "Completed" && 
                                                     t.CompletedDate != null).ToList();

                if (completedTasks.Any())
                {
                    var totalHours = completedTasks.Sum(t => (t.CompletedDate!.Value - t.CreatedAt).TotalHours);
                    statistics.AverageCompletionTime = totalHours / completedTasks.Count;
                }

                // Calculate completion rate
                if (taskData.Any())
                {
                    statistics.CompletionRate = (double)statistics.CompletedTasks / statistics.TotalTasks * 100;
                }

                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task statistics");
                throw;
            }
        }

        public async Task<IEnumerable<TaskCalendarDto>> GetTasksForCalendarAsync(Guid companyId, int year, int month, Guid userId)
        {
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var tasks = await _unitOfWork.Tasks.Query()
                    .Include(t => t.AssignedTo)
                    .Where(t => t.CompanyId == companyId &&
                               !t.IsArchived &&
                               (
                                   (t.StartDate != null && t.StartDate >= startDate && t.StartDate <= endDate) ||
                                   (t.DueDate != null && t.DueDate >= startDate && t.DueDate <= endDate) ||
                                   (t.StartDate != null && t.DueDate != null && t.StartDate <= startDate && t.DueDate >= endDate)
                               ))
                    .ToListAsync();

                var calendarTasks = tasks.Select(t => new TaskCalendarDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    StartDate = t.StartDate,
                    DueDate = t.DueDate,
                    Priority = t.Priority ?? "Medium",
                    Status = t.Status ?? "Pending",
                    AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : null,
                    Color = GetTaskColor(t.Priority, t.Status)
                });

                return calendarTasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting calendar tasks");
                throw;
            }
        }

        private string GetTaskColor(string? priority, string? status)
        {
            // Colors based on priority
            if (status == "Completed")
                return "#4CAF50"; // Green

            return priority?.ToLower() switch
            {
                "critical" => "#F44336", // Red
                "high" => "#FF9800",     // Orange
                "medium" => "#2196F3",   // Blue
                "low" => "#9E9E9E",      // Gray
                _ => "#2196F3"           // Default Blue
            };
        }

        public async Task<TaskCommentDto?> AddCommentAsync(CreateTaskCommentDto commentDto)
        {
            try
            {
                // Verify task exists and belongs to the company
                var task = await _unitOfWork.Tasks.Query()
                    .FirstOrDefaultAsync(t => t.Id == commentDto.TaskId && t.CompanyId == commentDto.CompanyId);

                if (task == null)
                    return null;

                var comment = new TaskComment
                {
                    TaskId = commentDto.TaskId,
                    Comment = commentDto.Comment,
                    CreatedById = commentDto.UserId,
                    IsInternal = false
                };

                await _unitOfWork.TaskComments.AddAsync(comment);
                await _unitOfWork.SaveChangesAsync();

                // Reload with navigation properties
                var savedComment = await _unitOfWork.TaskComments.Query()
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == comment.Id);

                return _mapper.Map<TaskCommentDto>(savedComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment to task");
                throw;
            }
        }

        public async Task<TaskAttachmentDto?> AddAttachmentAsync(Guid taskId, Guid companyId, Guid userId, IFormFile file)
        {
            try
            {
                // Verify task exists and belongs to the company
                var task = await _unitOfWork.Tasks.Query()
                    .FirstOrDefaultAsync(t => t.Id == taskId && t.CompanyId == companyId);

                if (task == null)
                    return null;

                // Upload file using FileService
                var folderPath = Path.Combine("tasks", taskId.ToString());
                var fileUrl = await _fileService.UploadFileAsync(file, folderPath);

                var attachment = new TaskAttachment
                {
                    TaskId = taskId,
                    FileName = file.FileName,
                    FileUrl = fileUrl,
                    FileSize = file.Length,
                    FileType = file.ContentType,
                    UploadedById = userId,
                    CreatedById = userId,
                    UpdatedById = userId
                };

                await _unitOfWork.TaskAttachments.AddAsync(attachment);
                await _unitOfWork.SaveChangesAsync();

                // Reload with navigation properties
                var savedAttachment = await _unitOfWork.TaskAttachments.Query()
                    .Include(a => a.UploadedBy)
                    .FirstOrDefaultAsync(a => a.Id == attachment.Id);

                return _mapper.Map<TaskAttachmentDto>(savedAttachment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding attachment to task");
                throw;
            }
        }
    }
}