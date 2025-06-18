using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.API.Services.Interfaces;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Services.Implementation
{
    public class ClientService : IClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ClientService> _logger;
        private readonly ICurrentUserService _currentUserService;

        public ClientService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ClientService> logger,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<ClientResponseDto>> GetClientsAsync(
            Guid companyId, int pageNumber, int pageSize, string? search, 
            string? status, string? sortBy, bool sortDescending)
        {
            try
            {
                // SuperAdmin can see all clients, others need CompanyId
                var currentCompanyId = _currentUserService.CompanyId;
                if (currentCompanyId == null && _currentUserService.UserRole != "SuperAdmin")
                    throw new UnauthorizedAccessException("CompanyId not found");
                    
                IQueryable<Client> query = _unitOfWork.Clients.Query()
                    .Include(c => c.Projects)
                    .Include(c => c.Tasks);

                // Filter by company only if not SuperAdmin
                if (_currentUserService.UserRole != "SuperAdmin" && currentCompanyId.HasValue)
                {
                    query = query.Where(c => c.CompanyId == currentCompanyId.Value);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(c => c.Name.Contains(search) ||
                                           c.Email.Contains(search) ||
                                           c.Phone.Contains(search));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    // Filter by IsActive status - "Active" or "Inactive"
                    if (status.ToLower() == "active")
                        query = query.Where(c => c.IsActive);
                    else if (status.ToLower() == "inactive")
                        query = query.Where(c => !c.IsActive);
                }

                // Apply sorting
                query = sortBy?.ToLower() switch
                {
                    "name" => sortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                    "email" => sortDescending ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),
                    "createdat" => sortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                    _ => sortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt)
                };

                var totalCount = await query.CountAsync();

                var clients = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var clientDtos = _mapper.Map<List<ClientResponseDto>>(clients);

                return new PagedResult<ClientResponseDto>
                {
                    Items = clientDtos,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all clients");
                throw;
            }
        }

        public async Task<ClientResponseDto?> GetClientByIdAsync(Guid clientId, Guid companyId)
        {
            try
            {
                var currentCompanyId = _currentUserService.CompanyId;
                if (currentCompanyId == null && _currentUserService.UserRole != "SuperAdmin")
                    throw new UnauthorizedAccessException("CompanyId not found");

                var query = _unitOfWork.Clients.Query()
                    .Include(c => c.Projects)
                    .Include(c => c.Tasks)
                    .Where(c => c.Id == clientId);

                // Filter by company only if not SuperAdmin
                if (_currentUserService.UserRole != "SuperAdmin")
                {
                    query = query.Where(c => c.CompanyId == companyId);
                }

                var client = await query.FirstOrDefaultAsync();

                return client == null ? null : _mapper.Map<ClientResponseDto>(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client by ID: {ClientId}", clientId);
                throw;
            }
        }

        public async Task<ClientResponseDto> CreateClientAsync(CreateClientDto createClientDto)
        {
            try
            {
                var client = _mapper.Map<Client>(createClientDto);
                client.CreatedById = _currentUserService.UserId;
                client.UpdatedById = _currentUserService.UserId;

                await _unitOfWork.Clients.AddAsync(client);
                await _unitOfWork.SaveChangesAsync();

                return await GetClientByIdAsync(client.Id, client.CompanyId) ?? throw new InvalidOperationException("Failed to get created client");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client");
                throw;
            }
        }

        public async Task<ClientResponseDto?> UpdateClientAsync(Guid clientId, UpdateClientDto updateClientDto)
        {
            try
            {
                var companyId = _currentUserService.CompanyId;
                if (companyId == null && _currentUserService.UserRole != "SuperAdmin")
                    throw new UnauthorizedAccessException("CompanyId not found");

                var query = _unitOfWork.Clients.Query().Where(c => c.Id == clientId);

                // Filter by company only if not SuperAdmin
                if (_currentUserService.UserRole != "SuperAdmin" && companyId.HasValue)
                {
                    query = query.Where(c => c.CompanyId == companyId.Value);
                }

                var client = await query.FirstOrDefaultAsync();

                if (client == null)
                    throw new KeyNotFoundException($"Client with ID {clientId} not found");

                _mapper.Map(updateClientDto, client);
                client.UpdatedById = _currentUserService.UserId;
                client.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Clients.Update(client);
                await _unitOfWork.SaveChangesAsync();

                return await GetClientByIdAsync(client.Id, client.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating client: {ClientId}", clientId);
                throw;
            }
        }

        public async Task<bool> DeleteClientAsync(Guid clientId, Guid companyId)
        {
            try
            {
                var currentCompanyId = _currentUserService.CompanyId;
                if (currentCompanyId == null && _currentUserService.UserRole != "SuperAdmin")
                    throw new UnauthorizedAccessException("CompanyId not found");

                var query = _unitOfWork.Clients.Query().Where(c => c.Id == clientId);

                // Filter by company only if not SuperAdmin
                if (_currentUserService.UserRole != "SuperAdmin")
                {
                    query = query.Where(c => c.CompanyId == companyId);
                }

                var client = await query.FirstOrDefaultAsync();

                if (client == null)
                    return false;

                client.IsDeleted = true;
                client.UpdatedAt = DateTime.UtcNow;
                client.UpdatedById = _currentUserService.UserId;

                _unitOfWork.Clients.Update(client);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting client: {ClientId}", clientId);
                throw;
            }
        }

        public async Task<IEnumerable<ProjectResponseDto>> GetClientProjectsAsync(Guid clientId, Guid companyId)
        {
            try
            {
                var currentCompanyId = _currentUserService.CompanyId;
                if (currentCompanyId == null && _currentUserService.UserRole != "SuperAdmin")
                    throw new UnauthorizedAccessException("CompanyId not found");

                var query = _unitOfWork.Projects.Query()
                    .Include(p => p.ProjectManager)
                    .Include(p => p.Client)
                    .Where(p => p.ClientId == clientId);

                // Filter by company only if not SuperAdmin
                if (_currentUserService.UserRole != "SuperAdmin")
                {
                    query = query.Where(p => p.CompanyId == companyId);
                }

                var projects = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();

                return _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client projects: {ClientId}", clientId);
                throw;
            }
        }

        public async Task<IEnumerable<TaskResponseDto>> GetClientTasksAsync(Guid clientId, Guid companyId)
        {
            try
            {
                var currentCompanyId = _currentUserService.CompanyId;
                if (currentCompanyId == null && _currentUserService.UserRole != "SuperAdmin")
                    throw new UnauthorizedAccessException("CompanyId not found");

                var query = _unitOfWork.Tasks.Query()
                    .Include(t => t.AssignedTo)
                    .Include(t => t.AssignedBy)
                    .Include(t => t.Client)
                    .Include(t => t.Project)
                    .Where(t => t.ClientId == clientId);

                // Filter by company only if not SuperAdmin
                if (_currentUserService.UserRole != "SuperAdmin")
                {
                    query = query.Where(t => t.CompanyId == companyId);
                }

                var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();

                return _mapper.Map<IEnumerable<TaskResponseDto>>(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client tasks: {ClientId}", clientId);
                throw;
            }
        }
    }
}