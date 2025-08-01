using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.API.Services.Interfaces;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using BCryptNet = BCrypt.Net.BCrypt;

namespace TaskManagement.API.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly ICurrentUserService _currentUserService;

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UserService> logger,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<UserResponseDto>> GetAllUsersAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            Guid? companyId = null,
            string? role = null,
            string? department = null,
            bool? isActive = null,
            string? sortBy = "CreatedAt",
            bool sortDescending = true)
        {
            try
            {
                var query = _unitOfWork.Users.Query();

                // Apply company filter
                if (companyId.HasValue)
                {
                    query = query.Where(u => u.CompanyId == companyId);
                }

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(u => 
                        u.FirstName.Contains(searchTerm) ||
                        u.LastName.Contains(searchTerm) ||
                        u.Email.Contains(searchTerm));
                }

                // Apply role filter
                if (!string.IsNullOrEmpty(role))
                {
                    query = query.Where(u => u.Role == role);
                }

                // Apply department filter
                if (!string.IsNullOrEmpty(department))
                {
                    query = query.Where(u => u.Department == department);
                }

                // Apply active status filter
                if (isActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == isActive.Value);
                }

                // Apply sorting
                query = ApplySorting(query, sortBy, sortDescending);

                var totalCount = await query.CountAsync();

                var users = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(u => u.Company)
                    .ToListAsync();

                var userDtos = _mapper.Map<List<UserResponseDto>>(users);

                return new PagedResult<UserResponseDto>
                {
                    Items = userDtos,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filtered users");
                throw;
            }
        }

        private IQueryable<User> ApplySorting(IQueryable<User> query, string? sortBy, bool descending)
        {
            return sortBy?.ToLower() switch
            {
                "firstname" => descending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
                "lastname" => descending ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
                "email" => descending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "role" => descending ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
                "department" => descending ? query.OrderByDescending(u => u.Department) : query.OrderBy(u => u.Department),
                _ => descending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt)
            };
        }

        public async Task<UserResponseDto> GetUserByIdAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.Query()
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found");

                return _mapper.Map<UserResponseDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                // Check if email is unique
                var isEmailUnique = await _unitOfWork.Users.IsEmailUniqueAsync(createUserDto.Email);
                if (!isEmailUnique)
                    throw new InvalidOperationException($"Email {createUserDto.Email} is already in use");

                // Check if company can add more users
                var canAddUsers = await _unitOfWork.Companies.Query()
                    .Where(c => c.Id == createUserDto.CompanyId)
                    .Select(c => c.MaxUsers > c.Users.Count(u => u.IsActive))
                    .FirstOrDefaultAsync();

                if (!canAddUsers)
                    throw new InvalidOperationException("Company has reached maximum user limit");

                var user = _mapper.Map<User>(createUserDto);
                user.PasswordSalt = BCryptNet.GenerateSalt();
                user.PasswordHash = BCryptNet.HashPassword(createUserDto.Password, user.PasswordSalt);
                user.CreatedById = _currentUserService.UserId;
                user.UpdatedById = _currentUserService.UserId;

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return await GetUserByIdAsync(user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw;
            }
        }

        public async Task<UserResponseDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found");

                _mapper.Map(updateUserDto, user);
                user.UpdatedById = _currentUserService.UserId;
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return await GetUserByIdAsync(user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", userId);
                throw;
            }
        }

        public async Task<UserResponseDto> UpdateProfileAsync(Guid userId, UpdateProfileDto profileDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found");

                // Update only profile-specific fields (preserve role, active status, etc.)
                user.FirstName = profileDto.FirstName;
                user.LastName = profileDto.LastName;
                user.PhoneNumber = profileDto.PhoneNumber;
                user.Department = profileDto.Department;
                user.JobTitle = profileDto.JobTitle;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedById = _currentUserService.UserId;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return await GetUserByIdAsync(user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found");

                user.IsDeleted = true;
                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedById = _currentUserService.UserId;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ActivateUserAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found");

                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedById = _currentUserService.UserId;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> DeactivateUserAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found");

                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedById = _currentUserService.UserId;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user: {UserId}", userId);
                throw;
            }
        }

        public async Task<UserResponseDto> UpdateUserRoleAsync(Guid userId, string newRole)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found");

                user.Role = newRole;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedById = _currentUserService.UserId;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return await GetUserByIdAsync(user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user role: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<TaskResponseDto>> GetUserTasksAsync(Guid userId)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                var tasks = await _unitOfWork.Tasks.GetTasksByUserAsync(userId, companyId);
                return _mapper.Map<IEnumerable<TaskResponseDto>>(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user tasks: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<UserPermissionDto>> GetUserPermissionsAsync(Guid userId)
        {
            try
            {
                var permissions = await _unitOfWork.UserPermissions.Query()
                    .Include(p => p.GrantedBy)
                    .Where(p => p.UserId == userId)
                    .ToListAsync();

                return permissions.Select(p => new UserPermissionDto
                {
                    Permission = p.PermissionType,
                    Description = p.PermissionValue,
                    IsGranted = true,
                    Category = "Custom"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user permissions: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UpdateUserPermissionsAsync(Guid userId, UpdateUserPermissionsDto permissions)
        {
            try
            {
                // Remove existing permissions
                var existingPermissions = await _unitOfWork.UserPermissions.Query()
                    .Where(p => p.UserId == userId)
                    .ToListAsync();

                foreach (var permission in existingPermissions)
                {
                    _unitOfWork.UserPermissions.Remove(permission);
                }

                // Add new permissions
                foreach (var permissionName in permissions.Permissions)
                {
                    var userPermission = new UserPermission
                    {
                        UserId = userId,
                        PermissionType = permissionName,
                        PermissionValue = "Granted",
                        GrantedById = _currentUserService.UserId,
                        GrantedAt = DateTime.UtcNow,
                        CreatedById = _currentUserService.UserId,
                        UpdatedById = _currentUserService.UserId
                    };
                    await _unitOfWork.UserPermissions.AddAsync(userPermission);
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user permissions: {UserId}", userId);
                throw;
            }
        }

        public async Task<string> UpdateAvatarAsync(Guid userId, IFormFile avatar)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found");

                // In a real implementation, you would upload the file to a storage service
                // For now, we'll just return a placeholder URL
                var fileName = $"{userId}_{avatar.FileName}";
                var avatarUrl = $"/uploads/avatars/{fileName}";

                user.ProfileImageUrl = avatarUrl;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedById = _currentUserService.UserId;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return avatarUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user avatar: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(string role)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                var users = await _unitOfWork.Users.GetUsersByRoleAsync(role, companyId);
                return _mapper.Map<IEnumerable<UserResponseDto>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by role: {Role}", role);
                throw;
            }
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersByCompanyAsync(Guid companyId)
        {
            try
            {
                var users = await _unitOfWork.Users.GetUsersByCompanyAsync(companyId);
                return _mapper.Map<IEnumerable<UserResponseDto>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by company: {CompanyId}", companyId);
                throw;
            }
        }
    }
}