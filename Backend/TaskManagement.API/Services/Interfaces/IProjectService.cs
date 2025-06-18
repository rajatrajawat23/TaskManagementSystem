using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;

namespace TaskManagement.API.Services.Interfaces
{
    public interface IProjectService
    {
        Task<PagedResult<ProjectResponseDto>> GetProjectsAsync(
            Guid companyId, int pageNumber, int pageSize, string? search, 
            string? status, Guid? clientId, Guid? projectManagerId, 
            string? sortBy, bool sortDescending);
        Task<ProjectResponseDto?> GetProjectByIdAsync(Guid projectId, Guid companyId);
        Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto);
        Task<ProjectResponseDto?> UpdateProjectAsync(Guid projectId, UpdateProjectDto updateProjectDto);
        Task<bool> DeleteProjectAsync(Guid projectId, Guid companyId);
        Task<IEnumerable<TaskResponseDto>> GetProjectTasksAsync(Guid projectId, Guid companyId);
        Task<ProjectResponseDto?> AddTeamMemberAsync(AddTeamMemberDto dto);
        Task<ProjectResponseDto?> RemoveTeamMemberAsync(Guid projectId, Guid userId, Guid companyId, Guid updatedById);
        Task<ProjectStatisticsDto> GetProjectStatisticsAsync(Guid projectId, Guid companyId);
    }
}