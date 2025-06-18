using System.Security.Claims;
using TaskManagement.Core.Entities;

namespace TaskManagement.API.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        bool ValidateToken(string token);
    }
}