using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.API.Services.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IDashboardService dashboardService,
            ICurrentUserService currentUserService,
            ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardDto>> GetDashboard(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)        {
            try
            {
                var companyId = _currentUserService.CompanyId;
                var userId = _currentUserService.UserId;
                var userRole = _currentUserService.UserRole;

                var dashboard = await _dashboardService.GetDashboardAsync(
                    companyId, userId, userRole, startDate, endDate);
                
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard");
                return StatusCode(500, new { message = "An error occurred while retrieving dashboard data" });
            }
        }

        [HttpGet("company-overview")]
        [Authorize(Policy = "CompanyAdmin")]
        public async Task<ActionResult<CompanyOverviewDto>> GetCompanyOverview()
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");
                
                var overview = await _dashboardService.GetCompanyOverviewAsync(companyId);                return Ok(overview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company overview");
                return StatusCode(500, new { message = "An error occurred while retrieving company overview" });
            }
        }

        [HttpGet("system-overview")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<ActionResult<SystemOverviewDto>> GetSystemOverview()
        {
            try
            {
                var overview = await _dashboardService.GetSystemOverviewAsync();
                return Ok(overview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting system overview");
                return StatusCode(500, new { message = "An error occurred while retrieving system overview" });
            }
        }

        [HttpGet("user-performance")]
        public async Task<ActionResult<UserPerformanceDto>> GetUserPerformance(
            [FromQuery] Guid? userId = null,            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var targetUserId = userId ?? _currentUserService.UserId ?? 
                    throw new UnauthorizedAccessException("UserId not found");
                
                // Check permissions
                if (userId.HasValue && userId != _currentUserService.UserId)
                {
                    if (!User.IsInRole("Manager") && !User.IsInRole("CompanyAdmin") && !User.IsInRole("SuperAdmin"))
                        return Forbid();
                }

                var companyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");

                var performance = await _dashboardService.GetUserPerformanceAsync(
                    targetUserId, companyId, startDate, endDate);
                
                return Ok(performance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user performance");
                return StatusCode(500, new { message = "An error occurred while retrieving user performance" });            }
        }

        [HttpGet("recent-activities")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetRecentActivities(
            [FromQuery] int count = 20)
        {
            try
            {
                var companyId = _currentUserService.CompanyId;
                var userId = _currentUserService.UserId;
                var userRole = _currentUserService.UserRole;

                var activities = await _dashboardService.GetRecentActivitiesAsync(
                    companyId, userId, userRole, count);
                
                return Ok(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent activities");
                return StatusCode(500, new { message = "An error occurred while retrieving recent activities" });
            }
        }
    }
}