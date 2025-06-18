using System.Security.Claims;
using TaskManagement.API.Services.Interfaces;

namespace TaskManagement.API.Services.Implementation
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return string.IsNullOrEmpty(userIdClaim) ? null : Guid.Parse(userIdClaim);
            }
        }

        public Guid? CompanyId
        {
            get
            {
                var companyIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("CompanyId")?.Value;
                if (string.IsNullOrEmpty(companyIdClaim))
                    return null;
                
                if (Guid.TryParse(companyIdClaim, out var companyId))
                    return companyId;
                    
                return null;
            }
        }

        public string UserRole
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            }
        }

        public string UserName
        {
            get
            {
                var firstName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value ?? "";
                var lastName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Surname)?.Value ?? "";
                return $"{firstName} {lastName}".Trim();
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
            }
        }
    }
}