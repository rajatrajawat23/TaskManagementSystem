using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;

namespace TaskManagement.API.Services.Interfaces
{
    public interface ITaskService
    {
        Task<PagedResult<TaskResponseDto>> GetTasksAsync(Guid? companyId, int pageNumber, int pageSize, 
            string? status, string? priority, string? assignedToId, DateTime? startDate, 
            DateTime? endDate, string? search, string? sortBy, bool sortDescending);
        Task<TaskResponseDto?> GetTaskByIdAsync(Guid taskId, Guid? companyId);
        Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto createTaskRequest);
        Task<TaskResponseDto?> UpdateTaskAsync(UpdateTaskDto updateTaskRequest);
        Task<bool> DeleteTaskAsync(Guid taskId, Guid companyId);
        Task<TaskResponseDto?> AssignTaskAsync(AssignTaskDto assignTaskDto);
        Task<TaskResponseDto?> UpdateTaskStatusAsync(UpdateTaskStatusDto updateStatusDto);
        Task<IEnumerable<TaskResponseDto>> GetUserTasksAsync(Guid userId, Guid companyId);
        Task<IEnumerable<TaskResponseDto>> GetOverdueTasksAsync(Guid companyId);
        Task<TaskResponseDto?> DuplicateTaskAsync(Guid taskId, Guid companyId, Guid userId);
        Task<TaskStatisticsDto> GetTaskStatisticsAsync(Guid? companyId);
        Task<IEnumerable<TaskCalendarDto>> GetTasksForCalendarAsync(Guid companyId, int year, int month, Guid userId);
        Task<TaskCommentDto?> AddCommentAsync(CreateTaskCommentDto commentDto);
        Task<TaskAttachmentDto?> AddAttachmentAsync(Guid taskId, Guid companyId, Guid userId, IFormFile file);
        Task<IEnumerable<TaskCommentDto>> GetTaskCommentsAsync(Guid taskId, Guid companyId);
        Task<IEnumerable<TaskAttachmentDto>> GetTaskAttachmentsAsync(Guid taskId, Guid companyId);
    }
}