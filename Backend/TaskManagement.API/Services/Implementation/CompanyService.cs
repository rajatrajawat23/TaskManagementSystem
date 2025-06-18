using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.API.Services.Interfaces;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Services.Implementation
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanyService> _logger;
        private readonly ICurrentUserService _currentUserService;

        public CompanyService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CompanyService> logger,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<CompanyResponseDto>> GetAllCompaniesAsync(
            int pageNumber, int pageSize, string? search, 
            string? subscriptionType, bool? isActive)
        {
            try
            {
                var query = _unitOfWork.Companies.Query()
                    .Include(c => c.Users)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(c => c.Name.Contains(search) || 
                                           c.Domain.Contains(search) ||
                                           c.ContactEmail.Contains(search));
                }
                
                if (!string.IsNullOrEmpty(subscriptionType))
                {
                    query = query.Where(c => c.SubscriptionType == subscriptionType);
                }
                
                if (isActive.HasValue)
                {
                    query = query.Where(c => c.IsActive == isActive.Value);
                }

                var totalCount = await query.CountAsync();

                var companies = await query
                    .OrderBy(c => c.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var companyDtos = _mapper.Map<List<CompanyResponseDto>>(companies);

                return new PagedResult<CompanyResponseDto>
                {
                    Items = companyDtos,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all companies");
                throw;
            }
        }

        public async Task<CompanyResponseDto> GetCompanyByIdAsync(Guid companyId)
        {
            try
            {
                var company = await _unitOfWork.Companies.Query()
                    .Include(c => c.Users)
                    .FirstOrDefaultAsync(c => c.Id == companyId);

                if (company == null)
                    throw new KeyNotFoundException($"Company with ID {companyId} not found");

                return _mapper.Map<CompanyResponseDto>(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company by ID: {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<CompanyResponseDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto)
        {
            try
            {
                // Check if domain is unique
                var isDomainUnique = await _unitOfWork.Companies.IsDomainUniqueAsync(createCompanyDto.Domain);
                if (!isDomainUnique)
                    throw new InvalidOperationException($"Domain {createCompanyDto.Domain} is already in use");

                var company = _mapper.Map<Company>(createCompanyDto);
                company.CreatedById = _currentUserService.UserId;
                company.UpdatedById = _currentUserService.UserId;

                await _unitOfWork.Companies.AddAsync(company);
                await _unitOfWork.SaveChangesAsync();

                return await GetCompanyByIdAsync(company.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company");
                throw;
            }
        }

        public async Task<CompanyResponseDto> UpdateCompanyAsync(Guid companyId, UpdateCompanyDto updateCompanyDto)
        {
            try
            {
                var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
                if (company == null)
                    throw new KeyNotFoundException($"Company with ID {companyId} not found");

                _mapper.Map(updateCompanyDto, company);
                company.UpdatedById = _currentUserService.UserId;
                company.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Companies.Update(company);
                await _unitOfWork.SaveChangesAsync();

                return await GetCompanyByIdAsync(company.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company: {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<bool> DeleteCompanyAsync(Guid companyId)
        {
            try
            {
                var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
                if (company == null)
                    throw new KeyNotFoundException($"Company with ID {companyId} not found");

                company.IsDeleted = true;
                company.UpdatedAt = DateTime.UtcNow;
                company.UpdatedById = _currentUserService.UserId;

                _unitOfWork.Companies.Update(company);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company: {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<bool> ActivateCompanyAsync(Guid companyId)
        {
            try
            {
                var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
                if (company == null)
                    throw new KeyNotFoundException($"Company with ID {companyId} not found");

                company.IsActive = true;
                company.UpdatedAt = DateTime.UtcNow;
                company.UpdatedById = _currentUserService.UserId;

                _unitOfWork.Companies.Update(company);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating company: {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<bool> DeactivateCompanyAsync(Guid companyId)
        {
            try
            {
                var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
                if (company == null)
                    throw new KeyNotFoundException($"Company with ID {companyId} not found");

                company.IsActive = false;
                company.UpdatedAt = DateTime.UtcNow;
                company.UpdatedById = _currentUserService.UserId;

                _unitOfWork.Companies.Update(company);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating company: {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<CompanyResponseDto> UpdateSubscriptionAsync(Guid companyId, UpdateSubscriptionDto updateSubscriptionDto)
        {
            try
            {
                var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
                if (company == null)
                    throw new KeyNotFoundException($"Company with ID {companyId} not found");

                company.SubscriptionType = updateSubscriptionDto.SubscriptionType;
                company.SubscriptionExpiryDate = updateSubscriptionDto.SubscriptionExpiryDate;
                company.MaxUsers = updateSubscriptionDto.MaxUsers;
                company.UpdatedAt = DateTime.UtcNow;
                company.UpdatedById = _currentUserService.UserId;

                _unitOfWork.Companies.Update(company);
                await _unitOfWork.SaveChangesAsync();

                return await GetCompanyByIdAsync(company.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription for company: {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<CompanyStatisticsDto> GetCompanyStatisticsAsync(Guid companyId)
        {
            try
            {
                var company = await _unitOfWork.Companies.Query()
                    .Include(c => c.Users)
                    .FirstOrDefaultAsync(c => c.Id == companyId);

                if (company == null)
                    throw new KeyNotFoundException($"Company with ID {companyId} not found");

                var statistics = new CompanyStatisticsDto
                {
                    CompanyId = company.Id,
                    CompanyName = company.Name,
                    TotalUsers = await _unitOfWork.Users.CountAsync(u => u.CompanyId == companyId),
                    ActiveUsers = await _unitOfWork.Users.CountAsync(u => u.CompanyId == companyId && u.IsActive),
                    MaxUsersAllowed = company.MaxUsers,
                    TotalProjects = await _unitOfWork.Projects.CountAsync(p => p.CompanyId == companyId),
                    ActiveProjects = await _unitOfWork.Projects.CountAsync(p => p.CompanyId == companyId && p.Status == "Active"),
                    CompletedProjects = await _unitOfWork.Projects.CountAsync(p => p.CompanyId == companyId && p.Status == "Completed"),
                    TotalTasks = await _unitOfWork.Tasks.CountAsync(t => t.CompanyId == companyId),
                    CompletedTasks = await _unitOfWork.Tasks.CountAsync(t => t.CompanyId == companyId && t.Status == "Completed"),
                    OverdueTasks = await _unitOfWork.Tasks.CountAsync(t => t.CompanyId == companyId && t.DueDate < DateTime.UtcNow && t.Status != "Completed"),
                    TotalClients = await _unitOfWork.Clients.CountAsync(c => c.CompanyId == companyId),
                    ActiveClients = await _unitOfWork.Clients.CountAsync(c => c.CompanyId == companyId && c.IsActive),
                    SubscriptionType = company.SubscriptionType,
                    SubscriptionExpiryDate = company.SubscriptionExpiryDate,
                    IsSubscriptionActive = company.IsActive && (!company.SubscriptionExpiryDate.HasValue || company.SubscriptionExpiryDate > DateTime.UtcNow)
                };

                // Calculate days until expiry
                if (company.SubscriptionExpiryDate.HasValue)
                {
                    statistics.DaysUntilExpiry = (int)(company.SubscriptionExpiryDate.Value - DateTime.UtcNow).TotalDays;
                }

                // Get user distribution by role
                var usersByRole = await _unitOfWork.Users.Query()
                    .Where(u => u.CompanyId == companyId)
                    .GroupBy(u => u.Role)
                    .Select(g => new { Role = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Role, x => x.Count);
                statistics.UsersByRole = usersByRole;

                // Get project distribution by status
                var projectsByStatus = await _unitOfWork.Projects.Query()
                    .Where(p => p.CompanyId == companyId)
                    .GroupBy(p => p.Status)
                    .Select(g => new { Status = g.Key ?? "Unknown", Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count);
                statistics.ProjectsByStatus = projectsByStatus;

                // Get task distribution by status
                var tasksByStatus = await _unitOfWork.Tasks.Query()
                    .Where(t => t.CompanyId == companyId)
                    .GroupBy(t => t.Status)
                    .Select(g => new { Status = g.Key ?? "Unknown", Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count);
                statistics.TasksByStatus = tasksByStatus;

                // Get task distribution by priority
                var tasksByPriority = await _unitOfWork.Tasks.Query()
                    .Where(t => t.CompanyId == companyId)
                    .GroupBy(t => t.Priority)
                    .Select(g => new { Priority = g.Key ?? "None", Count = g.Count() })
                    .ToDictionaryAsync(x => x.Priority, x => x.Count);
                statistics.TasksByPriority = tasksByPriority;

                // Get activity metrics for this month
                var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                statistics.ActiveTasksThisMonth = await _unitOfWork.Tasks.CountAsync(
                    t => t.CompanyId == companyId && t.CreatedAt >= startOfMonth);
                statistics.CompletedTasksThisMonth = await _unitOfWork.Tasks.CountAsync(
                    t => t.CompanyId == companyId && t.CompletedDate >= startOfMonth);

                // Get last activity date
                var lastTask = await _unitOfWork.Tasks.Query()
                    .Where(t => t.CompanyId == companyId)
                    .OrderByDescending(t => t.UpdatedAt)
                    .FirstOrDefaultAsync();
                statistics.LastActivityDate = lastTask?.UpdatedAt;

                // Calculate average task completion time
                var completedTasksWithTime = await _unitOfWork.Tasks.Query()
                    .Where(t => t.CompanyId == companyId && t.Status == "Completed" && t.CompletedDate != null)
                    .Select(t => new { t.CreatedAt, t.CompletedDate })
                    .ToListAsync();

                if (completedTasksWithTime.Any())
                {
                    var totalHours = completedTasksWithTime.Sum(t => (t.CompletedDate!.Value - t.CreatedAt).TotalHours);
                    statistics.AverageTaskCompletionTime = totalHours / completedTasksWithTime.Count;
                }

                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company statistics: {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<CompanyResponseDto?> UpdateSubscriptionAsync(UpdateSubscriptionDto dto)
        {
            try
            {
                var company = await _unitOfWork.Companies.GetByIdAsync(dto.CompanyId);
                if (company == null)
                    return null;

                company.SubscriptionType = dto.SubscriptionType;
                company.SubscriptionExpiryDate = dto.SubscriptionExpiryDate;
                company.MaxUsers = dto.MaxUsers;
                company.UpdatedAt = DateTime.UtcNow;
                company.UpdatedById = dto.UpdatedById;

                _unitOfWork.Companies.Update(company);
                await _unitOfWork.SaveChangesAsync();

                return await GetCompanyByIdAsync(company.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company subscription: {CompanyId}", dto.CompanyId);
                throw;
            }
        }
        
        public async Task<bool> CanAddMoreUsersAsync(Guid companyId)
        {
            try
            {
                var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
                if (company == null)
                    throw new KeyNotFoundException($"Company with ID {companyId} not found");

                var currentUserCount = await _unitOfWork.Users.CountAsync(u => u.CompanyId == companyId && u.IsActive);
                return currentUserCount < company.MaxUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if company can add more users: {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<IEnumerable<CompanyResponseDto>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry)
        {
            try
            {
                var expiryDate = DateTime.UtcNow.AddDays(daysBeforeExpiry);
                var companies = await _unitOfWork.Companies.Query()
                    .Where(c => c.SubscriptionExpiryDate.HasValue && 
                               c.SubscriptionExpiryDate.Value <= expiryDate &&
                               c.IsActive)
                    .OrderBy(c => c.SubscriptionExpiryDate)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<CompanyResponseDto>>(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expiring subscriptions");
                throw;
            }
        }
    }
}