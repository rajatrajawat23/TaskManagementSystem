using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;

namespace TaskManagement.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest);
        Task<bool> LogoutAsync(Guid userId);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto changePasswordRequest);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequest);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequest);
        Task<bool> VerifyEmailAsync(string token);
    }
}