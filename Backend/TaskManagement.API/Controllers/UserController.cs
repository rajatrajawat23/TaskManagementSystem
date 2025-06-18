using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.API.Services.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            ICurrentUserService currentUserService,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult<PagedResult<UserResponseDto>>> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? role = null,
            [FromQuery] string? department = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = "CreatedAt",
            [FromQuery] bool sortDescending = true)
        {
            try
            {
                var result = await _userService.GetAllUsersAsync(pageNumber, pageSize, search);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return StatusCode(500, new { message = "An error occurred while retrieving users" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(Guid id)
        {
            try
            {
                // Users can view their own profile or managers can view any user in their company
                if (id != _currentUserService.UserId && !User.IsInRole("Manager") && !User.IsInRole("CompanyAdmin"))
                    return Forbid();

                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the user" });
            }
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserResponseDto>> GetCurrentUserProfile()
        {
            try
            {
                var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user profile");
                return StatusCode(500, new { message = "An error occurred while retrieving your profile" });
            }
        }

        [HttpPut("profile")]
        public async Task<ActionResult<UserResponseDto>> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.UserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");
                dto.CompanyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");

                var updateDto = new UpdateUserDto
                {
                    Id = dto.UserId,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PhoneNumber = dto.PhoneNumber,
                    Department = dto.Department,
                    JobTitle = dto.JobTitle
                };

                var user = await _userService.UpdateUserAsync(dto.UserId, updateDto);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, new { message = "An error occurred while updating your profile" });
            }
        }

        [HttpPost]
        [Authorize(Policy = "CompanyAdmin")]
        public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] CreateUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.CompanyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                dto.CreatedById = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");

                var user = await _userService.CreateUserAsync(dto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, new { message = "An error occurred while creating the user" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "CompanyAdmin")]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.Id = id;
                dto.CompanyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                dto.UpdatedById = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");

                var user = await _userService.UpdateUserAsync(id, dto);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the user" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "CompanyAdmin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                if (id == _currentUserService.UserId)
                    return BadRequest(new { message = "You cannot delete your own account" });

                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                    return NotFound(new { message = "User not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the user" });
            }
        }

        [HttpPut("{id}/activate")]
        [Authorize(Policy = "CompanyAdmin")]
        public async Task<ActionResult<UserResponseDto>> ActivateUser(Guid id)
        {
            try
            {
                var result = await _userService.ActivateUserAsync(id);
                if (!result)
                    return NotFound(new { message = "User not found" });

                var user = await _userService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while activating the user" });
            }
        }

        [HttpPut("{id}/deactivate")]
        [Authorize(Policy = "CompanyAdmin")]
        public async Task<ActionResult<UserResponseDto>> DeactivateUser(Guid id)
        {
            try
            {
                if (id == _currentUserService.UserId)
                    return BadRequest(new { message = "You cannot deactivate your own account" });

                var result = await _userService.DeactivateUserAsync(id);
                if (!result)
                    return NotFound(new { message = "User not found" });

                var user = await _userService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while deactivating the user" });
            }
        }

        [HttpPut("{id}/role")]
        [Authorize(Policy = "CompanyAdmin")]
        public async Task<ActionResult<UserResponseDto>> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleDto dto)
        {
            try
            {
                dto.UserId = id;
                dto.CompanyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                dto.UpdatedById = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");

                var user = await _userService.UpdateUserRoleAsync(id, dto.Role);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user role {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while updating user role" });
            }
        }

        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetUserTasks(Guid id)
        {
            try
            {
                // Users can view their own tasks or managers can view any user's tasks
                if (id != _currentUserService.UserId && !User.IsInRole("Manager") && !User.IsInRole("CompanyAdmin"))
                    return Forbid();

                var tasks = await _userService.GetUserTasksAsync(id);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving user tasks" });
            }
        }

        [HttpGet("{id}/permissions")]
        [Authorize(Policy = "CompanyAdmin")]
        public async Task<ActionResult<IEnumerable<UserPermissionDto>>> GetUserPermissions(Guid id)
        {
            try
            {
                var permissions = await _userService.GetUserPermissionsAsync(id);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving user permissions" });
            }
        }

        [HttpPost("{id}/permissions")]
        [Authorize(Policy = "CompanyAdmin")]
        public async Task<ActionResult<UserResponseDto>> UpdateUserPermissions(Guid id, [FromBody] UpdateUserPermissionsDto dto)
        {
            try
            {
                dto.UserId = id;
                dto.CompanyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                dto.UpdatedById = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");

                var result = await _userService.UpdateUserPermissionsAsync(id, dto);
                var user = result ? await _userService.GetUserByIdAsync(id) : null;
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permissions for user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while updating user permissions" });
            }
        }

        [HttpPost("profile/avatar")]
        public async Task<ActionResult<UserResponseDto>> UploadAvatar([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "File is required" });

                var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");
                var avatarUrl = await _userService.UpdateAvatarAsync(userId, file);
                
                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading avatar");
                return StatusCode(500, new { message = "An error occurred while uploading avatar" });
            }
        }
    }
}