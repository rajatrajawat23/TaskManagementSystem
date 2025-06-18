using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;

namespace TaskManagement.API.Services.Interfaces
{
    public interface IClientService
    {
        Task<PagedResult<ClientResponseDto>> GetClientsAsync(
            Guid companyId, int pageNumber, int pageSize, string? search, 
            string? status, string? sortBy, bool sortDescending);
        Task<ClientResponseDto?> GetClientByIdAsync(Guid clientId, Guid companyId);
        Task<ClientResponseDto> CreateClientAsync(CreateClientDto createClientDto);
        Task<ClientResponseDto?> UpdateClientAsync(Guid clientId, UpdateClientDto updateClientDto);
        Task<bool> DeleteClientAsync(Guid clientId, Guid companyId);
        Task<IEnumerable<ProjectResponseDto>> GetClientProjectsAsync(Guid clientId, Guid companyId);
        Task<IEnumerable<TaskResponseDto>> GetClientTasksAsync(Guid clientId, Guid companyId);
    }
}