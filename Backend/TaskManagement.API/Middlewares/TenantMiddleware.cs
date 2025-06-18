using System.Security.Claims;

namespace TaskManagement.API.Middlewares
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var companyIdClaim = context.User.FindFirst("CompanyId")?.Value;
                if (!string.IsNullOrEmpty(companyIdClaim))
                {
                    context.Items["CompanyId"] = Guid.Parse(companyIdClaim);
                }

                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    context.Items["UserId"] = Guid.Parse(userIdClaim);
                }
            }

            await _next(context);
        }
    }
}