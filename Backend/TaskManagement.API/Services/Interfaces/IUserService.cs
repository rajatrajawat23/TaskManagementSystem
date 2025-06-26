using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;

namespace TaskManagement.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserResponseDto>> GetAllUsersAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            Guid? companyId = null,
            string? role = null,
            string? department = null,
            bool? isActive = null,
            string? sortBy = "CreatedAt",
            bool sortDescending = true);
        Task<UserResponseDto> GetUserByIdAsync(Guid userId);
        Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserResponseDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto);
        Task<UserResponseDto> UpdateProfileAsync(Guid userId, UpdateProfileDto profileDto);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> ActivateUserAsync(Guid userId);
        Task<bool> DeactivateUserAsync(Guid userId);
        Task<UserResponseDto> UpdateUserRoleAsync(Guid userId, string newRole);
        Task<IEnumerable<TaskResponseDto>> GetUserTasksAsync(Guid userId);
        Task<IEnumerable<UserPermissionDto>> GetUserPermissionsAsync(Guid userId);
        Task<bool> UpdateUserPermissionsAsync(Guid userId, UpdateUserPermissionsDto permissions);
        Task<string> UpdateAvatarAsync(Guid userId, IFormFile avatar);
        Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<UserResponseDto>> GetUsersByCompanyAsync(Guid companyId);
    }
}