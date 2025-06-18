using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.API.Services.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<ClientController> _logger;

        public ClientController(
            IClientService clientService,
            ICurrentUserService currentUserService,
            ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ClientResponseDto>>> GetClients(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] string? sortBy = "CreatedAt",
            [FromQuery] bool sortDescending = true)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");
                
                var result = await _clientService.GetClientsAsync(
                    companyId, pageNumber, pageSize, search, status, sortBy, sortDescending);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clients");
                return StatusCode(500, new { message = "An error occurred while retrieving clients" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientResponseDto>> GetClient(Guid id)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");
                
                var client = await _clientService.GetClientByIdAsync(id, companyId);
                if (client == null)
                    return NotFound(new { message = "Client not found" });

                return Ok(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client {ClientId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the client" });
            }
        }

        [HttpPost]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult<ClientResponseDto>> CreateClient([FromBody] CreateClientDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.CompanyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");
                dto.CreatedById = _currentUserService.UserId ?? 
                    throw new UnauthorizedAccessException("UserId not found");

                var client = await _clientService.CreateClientAsync(dto);
                return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client");
                return StatusCode(500, new { message = "An error occurred while creating the client" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult<ClientResponseDto>> UpdateClient(Guid id, [FromBody] UpdateClientDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.Id = id;
                dto.CompanyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");
                dto.UpdatedById = _currentUserService.UserId ?? 
                    throw new UnauthorizedAccessException("UserId not found");

                var client = await _clientService.UpdateClientAsync(id, dto);
                if (client == null)
                    return NotFound(new { message = "Client not found" });

                return Ok(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating client {ClientId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the client" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");
                
                var result = await _clientService.DeleteClientAsync(id, companyId);
                if (!result)
                    return NotFound(new { message = "Client not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting client {ClientId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the client" });
            }
        }

        [HttpGet("{id}/projects")]
        public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetClientProjects(Guid id)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");
                
                var projects = await _clientService.GetClientProjectsAsync(id, companyId);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects for client {ClientId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving client projects" });
            }
        }

        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetClientTasks(Guid id)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");
                
                var tasks = await _clientService.GetClientTasksAsync(id, companyId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for client {ClientId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving client tasks" });
            }
        }
    }
}