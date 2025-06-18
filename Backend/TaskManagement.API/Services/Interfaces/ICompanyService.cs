using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;

namespace TaskManagement.API.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<PagedResult<CompanyResponseDto>> GetAllCompaniesAsync(
            int pageNumber, int pageSize, string? search, 
            string? subscriptionType, bool? isActive);
        Task<CompanyResponseDto> GetCompanyByIdAsync(Guid companyId);
        Task<CompanyResponseDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto);
        Task<CompanyResponseDto> UpdateCompanyAsync(Guid companyId, UpdateCompanyDto updateCompanyDto);
        Task<bool> DeleteCompanyAsync(Guid companyId);
        Task<bool> ActivateCompanyAsync(Guid companyId);
        Task<bool> DeactivateCompanyAsync(Guid companyId);
        Task<CompanyResponseDto?> UpdateSubscriptionAsync(UpdateSubscriptionDto dto);
        Task<CompanyStatisticsDto> GetCompanyStatisticsAsync(Guid companyId);
        Task<bool> CanAddMoreUsersAsync(Guid companyId);
        Task<IEnumerable<CompanyResponseDto>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry);
    }
}