using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.API.Services.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public AuthController(
            IAuthService authService,
            IUserService userService,
            ICurrentUserService currentUserService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _userService = userService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                var result = await _authService.LoginAsync(loginRequest);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpPost("register")]
        [Authorize(Roles = "SuperAdmin,CompanyAdmin")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerRequest);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        [HttpPost("register-test-user")]
        public async Task<IActionResult> RegisterTestUser([FromBody] RegisterRequestDto registerRequest)
        {
            try
            {
                // This is a temporary endpoint for testing - should be removed in production
                var result = await _authService.RegisterAsync(registerRequest);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during test registration");
                return StatusCode(500, new { message = "An error occurred during test registration" });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequest)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshTokenRequest);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return StatusCode(500, new { message = "An error occurred while refreshing token" });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return BadRequest(new { message = "Invalid user authentication" });
                }
                
                await _authService.LogoutAsync(userId);
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto changePasswordRequest)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return BadRequest(new { message = "Invalid user authentication" });
                }
                
                var result = await _authService.ChangePasswordAsync(userId, changePasswordRequest);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to change password" });
                }

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, new { message = "An error occurred while changing password" });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordRequest)
        {
            try
            {
                var result = await _authService.ForgotPasswordAsync(forgotPasswordRequest);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to process forgot password request" });
                }

                return Ok(new { message = "Password reset instructions sent to your email" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing forgot password");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordRequest)
        {
            try
            {
                var result = await _authService.ResetPasswordAsync(resetPasswordRequest);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to reset password" });
                }

                return Ok(new { message = "Password reset successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return StatusCode(500, new { message = "An error occurred while resetting password" });
            }
        }

        [HttpGet("verify-email/{token}")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            try
            {
                var result = await _authService.VerifyEmailAsync(token);
                
                if (!result)
                {
                    return BadRequest(new { message = "Invalid or expired verification token" });
                }

                return Ok(new { message = "Email verified successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email");
                return StatusCode(500, new { message = "An error occurred while verifying email" });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = _currentUserService.UserId;
                if (userId == null)
                {
                    return Unauthorized(new { message = "Not authenticated" });
                }

                var user = await _userService.GetUserByIdAsync(userId.Value);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(500, new { message = "An error occurred while retrieving user information" });
            }
        }
    }
}