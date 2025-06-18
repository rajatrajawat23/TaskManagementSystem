using BCrypt.Net;
using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.API.Services.Interfaces;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            ILogger<AuthService> logger,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _logger = logger;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", loginRequest.Email);
                
                var user = await _unitOfWork.Users.FindAsync(u => u.Email == loginRequest.Email);
                var foundUser = user.FirstOrDefault();

                if (foundUser == null)
                {
                    _logger.LogWarning("User not found: {Email}", loginRequest.Email);
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                _logger.LogInformation("User found. Verifying password...");
                
                if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, foundUser.PasswordHash))
                {
                    _logger.LogWarning("Invalid password for user: {Email}", loginRequest.Email);
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                if (!foundUser.IsActive)
                {
                    _logger.LogWarning("Inactive account login attempt: {Email}", loginRequest.Email);
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Account is deactivated"
                    };
                }

                _logger.LogInformation("Password verified. Generating tokens...");

                var accessToken = _jwtService.GenerateAccessToken(foundUser);
                var refreshToken = _jwtService.GenerateRefreshToken();

                foundUser.RefreshToken = refreshToken;
                foundUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:RefreshTokenExpiryDays"]));
                foundUser.LastLoginAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(foundUser);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Login successful for user: {Email}", loginRequest.Email);

                var company = foundUser.CompanyId.HasValue 
                    ? await _unitOfWork.Companies.GetByIdAsync(foundUser.CompanyId.Value) 
                    : null;

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Login successful",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpiryMinutes"])),
                    User = new UserDto
                    {
                        Id = foundUser.Id,
                        Email = foundUser.Email,
                        FirstName = foundUser.FirstName,
                        LastName = foundUser.LastName,
                        Role = foundUser.Role,
                        CompanyId = foundUser.CompanyId,
                        CompanyName = company?.Name,
                        ProfileImageUrl = foundUser.ProfileImageUrl
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", loginRequest.Email);
                throw;
            }
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _unitOfWork.Users.ExistsAsync(u => u.Email == registerRequest.Email);
                if (existingUser)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User with this email already exists"
                    };
                }

                // Create new user
                var passwordSalt = BCrypt.Net.BCrypt.GenerateSalt();
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

                var newUser = new User
                {
                    Email = registerRequest.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    FirstName = registerRequest.FirstName,
                    LastName = registerRequest.LastName,
                    PhoneNumber = registerRequest.PhoneNumber,
                    CompanyId = registerRequest.CompanyId,
                    Role = registerRequest.Role,
                    Department = registerRequest.Department,
                    JobTitle = registerRequest.JobTitle,
                    IsActive = true,
                    EmailVerified = false
                };

                await _unitOfWork.Users.AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();

                // Generate email verification token
                var emailVerificationToken = Guid.NewGuid().ToString();
                newUser.PasswordResetToken = emailVerificationToken; // Reusing the field for email verification
                newUser.PasswordResetExpiry = DateTime.UtcNow.AddHours(24);
                
                _unitOfWork.Users.Update(newUser);
                await _unitOfWork.SaveChangesAsync();

                // Send verification email
                try
                {
                    await _emailService.SendEmailVerificationAsync(newUser.Email, emailVerificationToken);
                    _logger.LogInformation("Verification email sent to {Email}", newUser.Email);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send verification email to {Email}", newUser.Email);
                }

                // Send welcome email
                try
                {
                    var userCompany = newUser.CompanyId.HasValue 
                        ? await _unitOfWork.Companies.GetByIdAsync(newUser.CompanyId.Value) 
                        : null;
                    
                    await _emailService.SendWelcomeEmailAsync(
                        newUser.Email, 
                        $"{newUser.FirstName} {newUser.LastName}",
                        userCompany?.Name ?? "Task Management System");
                    _logger.LogInformation("Welcome email sent to {Email}", newUser.Email);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send welcome email to {Email}", newUser.Email);
                }

                // Generate tokens
                var accessToken = _jwtService.GenerateAccessToken(newUser);
                var refreshToken = _jwtService.GenerateRefreshToken();

                newUser.RefreshToken = refreshToken;
                newUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:RefreshTokenExpiryDays"]));

                _unitOfWork.Users.Update(newUser);
                await _unitOfWork.SaveChangesAsync();

                var company = newUser.CompanyId.HasValue 
                    ? await _unitOfWork.Companies.GetByIdAsync(newUser.CompanyId.Value) 
                    : null;

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Registration successful",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpiryMinutes"])),
                    User = new UserDto
                    {
                        Id = newUser.Id,
                        Email = newUser.Email,
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        Role = newUser.Role,
                        CompanyId = newUser.CompanyId,
                        CompanyName = company?.Name,
                        ProfileImageUrl = newUser.ProfileImageUrl
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", registerRequest.Email);
                throw;
            }
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest)
        {
            try
            {
                var principal = _jwtService.GetPrincipalFromExpiredToken(refreshTokenRequest.AccessToken);
                var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                
                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
                {
                    _logger.LogWarning("Refresh token claim not found or empty");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid token"
                    };
                }
                
                _logger.LogInformation("Refresh token claim value: {ClaimValue}", userIdClaim.Value);
                
                if (!Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    _logger.LogWarning("Invalid user ID format in token: {ClaimValue}", userIdClaim.Value);
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid token format"
                    };
                }
                
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null || user.RefreshToken != refreshTokenRequest.RefreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid refresh token"
                    };
                }

                var newAccessToken = _jwtService.GenerateAccessToken(user);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:RefreshTokenExpiryDays"]));

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpiryMinutes"]))
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                throw;
            }
        }

        public async Task<bool> LogoutAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiry = null;
                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for userId: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto changePasswordRequest)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                if (!BCrypt.Net.BCrypt.Verify(changePasswordRequest.CurrentPassword, user.PasswordHash))
                {
                    return false;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordRequest.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for userId: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequest)
        {
            try
            {
                var users = await _unitOfWork.Users.FindAsync(u => u.Email == forgotPasswordRequest.Email);
                var user = users.FirstOrDefault();

                if (user == null)
                {
                    // Don't reveal if email exists
                    return true;
                }

                // Generate password reset token
                var resetToken = Guid.NewGuid().ToString();
                user.PasswordResetToken = resetToken;
                user.PasswordResetExpiry = DateTime.UtcNow.AddHours(24);

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                // Send password reset email
                try
                {
                    await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken);
                    _logger.LogInformation("Password reset email sent to {Email}", user.Email);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send password reset email to {Email}", user.Email);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing forgot password for email: {Email}", forgotPasswordRequest.Email);
                throw;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequest)
        {
            try
            {
                var users = await _unitOfWork.Users.FindAsync(u => 
                    u.PasswordResetToken == resetPasswordRequest.Token && 
                    u.PasswordResetExpiry > DateTime.UtcNow);
                
                var user = users.FirstOrDefault();
                if (user == null)
                {
                    return false;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordRequest.NewPassword);
                user.PasswordResetToken = null;
                user.PasswordResetExpiry = null;
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                throw;
            }
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            try
            {
                var users = await _unitOfWork.Users.FindAsync(u => 
                    u.PasswordResetToken == token && 
                    u.PasswordResetExpiry > DateTime.UtcNow &&
                    !u.EmailVerified);
                
                var user = users.FirstOrDefault();
                if (user == null)
                {
                    _logger.LogWarning("Invalid or expired email verification token: {Token}", token);
                    return false;
                }

                user.EmailVerified = true;
                user.PasswordResetToken = null;
                user.PasswordResetExpiry = null;
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Email verified successfully for user: {Email}", user.Email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email");
                throw;
            }
        }
    }
}