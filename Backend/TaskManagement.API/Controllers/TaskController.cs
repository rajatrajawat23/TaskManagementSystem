using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.API.Services.Interfaces;
using System.Security.Claims;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(
            ITaskService taskService, 
            ICurrentUserService currentUserService,
            ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<TaskResponseDto>>> GetTasks(
            [FromQuery] Guid? companyId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? priority = null,
            [FromQuery] string? assignedToId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = "CreatedAt",
            [FromQuery] bool sortDescending = true)
        {
            try
            {
                // SuperAdmin can query any company or all companies, others can only query their own
                Guid? targetCompanyId = null;
                if (_currentUserService.UserRole == "SuperAdmin")
                {
                    // SuperAdmin can specify a company or see all companies
                    targetCompanyId = companyId; // null means all companies
                }
                else
                {
                    // Regular users can only see their own company
                    targetCompanyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                }
                
                var result = await _taskService.GetTasksAsync(
                    targetCompanyId, pageNumber, pageSize, status, priority, 
                    assignedToId, startDate, endDate, search, sortBy, sortDescending);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks");
                return StatusCode(500, new { message = "An error occurred while retrieving tasks" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponseDto>> GetTask(Guid id)
        {
            try
            {
                // SuperAdmin can get any task, others can only get tasks from their company
                Guid? companyId = null;
                if (_currentUserService.UserRole != "SuperAdmin")
                {
                    companyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                }
                
                var task = await _taskService.GetTaskByIdAsync(id, companyId);
                if (task == null)
                    return NotFound(new { message = "Task not found" });

                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task {TaskId}", id);
                return StatusCode(500, new { 
                    message = "An error occurred while retrieving the task",
                    detail = ex.Message,
                    innerDetail = ex.InnerException?.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult<TaskResponseDto>> CreateTask([FromBody] CreateTaskDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // For SuperAdmin, require CompanyId in the request
                if (_currentUserService.UserRole == "SuperAdmin")
                {
                    if (dto.CompanyId == Guid.Empty)
                        return BadRequest(new { message = "CompanyId is required for SuperAdmin" });
                }
                else
                {
                    dto.CompanyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                }
                
                dto.AssignedById = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");

                var task = await _taskService.CreateTaskAsync(dto);
                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, new { message = "An error occurred while creating the task" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult<TaskResponseDto>> UpdateTask(Guid id, [FromBody] UpdateTaskDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.Id = id;
                dto.UpdatedById = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");

                // For SuperAdmin, the service will handle CompanyId from existing task
                if (_currentUserService.UserRole != "SuperAdmin")
                {
                    dto.CompanyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                }

                var task = await _taskService.UpdateTaskAsync(dto);
                if (task == null)
                    return NotFound(new { message = "Task not found" });

                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the task" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            try
            {
                // For SuperAdmin, pass empty Guid to indicate no company filter
                var companyId = _currentUserService.UserRole == "SuperAdmin" 
                    ? Guid.Empty 
                    : _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                    
                var result = await _taskService.DeleteTaskAsync(id, companyId);
                if (!result)
                    return NotFound(new { message = "Task not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the task" });
            }
        }

        [HttpPost("{id}/assign")]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult<TaskResponseDto>> AssignTask(Guid id, [FromBody] AssignTaskDto dto)
        {
            try
            {
                dto.TaskId = id;
                dto.CompanyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                dto.AssignedById = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");

                var task = await _taskService.AssignTaskAsync(dto);
                if (task == null)
                    return NotFound(new { message = "Task not found" });

                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task {TaskId}", id);
                return StatusCode(500, new { message = "An error occurred while assigning the task" });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<TaskResponseDto>> UpdateTaskStatus(Guid id, [FromBody] UpdateTaskStatusDto dto)
        {
            try
            {
                dto.TaskId = id;
                dto.UpdatedById = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");
                
                // For SuperAdmin, the service will handle CompanyId from existing task
                if (_currentUserService.UserRole != "SuperAdmin")
                {
                    dto.CompanyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                }

                var task = await _taskService.UpdateTaskStatusAsync(dto);
                if (task == null)
                    return NotFound(new { message = "Task not found" });

                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task status {TaskId}", id);
                return StatusCode(500, new { message = "An error occurred while updating task status" });
            }
        }

        [HttpGet("calendar/{year}/{month}")]
        public async Task<ActionResult<IEnumerable<TaskCalendarDto>>> GetTasksCalendar(int year, int month)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");
                var tasks = await _taskService.GetTasksForCalendarAsync(
                    companyId, year, month, userId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting calendar tasks for {Year}/{Month}", year, month);
                return StatusCode(500, new { message = "An error occurred while retrieving calendar tasks" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetUserTasks(Guid userId)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                var tasks = await _taskService.GetUserTasksAsync(userId, companyId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving user tasks" });
            }
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetOverdueTasks()
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                var tasks = await _taskService.GetOverdueTasksAsync(companyId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting overdue tasks");
                return StatusCode(500, new { message = "An error occurred while retrieving overdue tasks" });
            }
        }

        [HttpPost("{id}/duplicate")]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult<TaskResponseDto>> DuplicateTask(Guid id)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");
                var task = await _taskService.DuplicateTaskAsync(
                    id, companyId, userId);
                
                if (task == null)
                    return NotFound(new { message = "Task not found" });

                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error duplicating task {TaskId}", id);
                return StatusCode(500, new { message = "An error occurred while duplicating the task" });
            }
        }

        [HttpGet("test")]
        public IActionResult TestEndpoint()
        {
            return Ok(new { message = "Test endpoint works", timestamp = DateTime.UtcNow });
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<TaskStatisticsDto>> GetTaskStatistics()
        {
            try
            {
                // SuperAdmin gets statistics for all companies, others get their company only
                Guid? companyId = null;
                if (_currentUserService.UserRole != "SuperAdmin")
                {
                    companyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                }
                
                var statistics = await _taskService.GetTaskStatisticsAsync(companyId);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task statistics");
                return StatusCode(500, new { 
                    message = "An error occurred while retrieving task statistics",
                    detail = ex.Message,
                    innerDetail = ex.InnerException?.Message
                });
            }
        }

        [HttpPost("{id}/comment")]
        public async Task<ActionResult<TaskCommentDto>> AddComment(Guid id, [FromBody] CreateTaskCommentDto dto)
        {
            try
            {
                dto.TaskId = id;
                dto.UserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");
                dto.CompanyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");

                var comment = await _taskService.AddCommentAsync(dto);
                if (comment == null)
                    return NotFound(new { message = "Task not found" });

                return Ok(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment to task {TaskId}", id);
                return StatusCode(500, new { message = "An error occurred while adding the comment" });
            }
        }

        [HttpPost("{id}/attachment")]
        [Authorize(Policy = "User")]
        public async Task<ActionResult<TaskAttachmentDto>> AddAttachment(Guid id, [FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "File is required" });

                var companyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");
                var attachment = await _taskService.AddAttachmentAsync(
                    id, companyId, userId, file);

                if (attachment == null)
                    return NotFound(new { message = "Task not found" });

                return Ok(attachment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding attachment to task {TaskId}", id);
                return StatusCode(500, new { message = "An error occurred while adding the attachment" });
            }
        }
    }
}