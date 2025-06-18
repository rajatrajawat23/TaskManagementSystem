using TaskManagement.API.Models.DTOs.Response;

namespace TaskManagement.API.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync(Guid? companyId, Guid? userId, string? userRole, 
            DateTime? startDate, DateTime? endDate);
        Task<CompanyOverviewDto> GetCompanyOverviewAsync(Guid companyId);
        Task<SystemOverviewDto> GetSystemOverviewAsync();
        Task<UserPerformanceDto> GetUserPerformanceAsync(Guid userId, Guid companyId, 
            DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<ActivityDto>> GetRecentActivitiesAsync(Guid? companyId, Guid? userId, 
            string? userRole, int count);
    }
}