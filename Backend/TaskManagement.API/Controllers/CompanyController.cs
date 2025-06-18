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
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(
            ICompanyService companyService,
            ICurrentUserService currentUserService,
            ILogger<CompanyController> logger)
        {
            _companyService = companyService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<ActionResult<PagedResult<CompanyResponseDto>>> GetCompanies(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? subscriptionType = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var result = await _companyService.GetAllCompaniesAsync(
                    pageNumber, pageSize, search, subscriptionType, isActive);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting companies");
                return StatusCode(500, new { message = "An error occurred while retrieving companies" });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "CompanyAdmin")]
        public async Task<ActionResult<CompanyResponseDto>> GetCompany(Guid id)
        {
            try
            {
                // SuperAdmin can view any company, others can only view their own
                if (_currentUserService.UserRole != "SuperAdmin" && id != _currentUserService.CompanyId)
                    return Forbid();

                var company = await _companyService.GetCompanyByIdAsync(id);
                if (company == null)
                    return NotFound(new { message = "Company not found" });

                return Ok(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company {CompanyId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the company" });
            }
        }

        [HttpGet("current")]
        public async Task<ActionResult<CompanyResponseDto>> GetCurrentCompany()
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");

                var company = await _companyService.GetCompanyByIdAsync(companyId);
                if (company == null)
                    return NotFound(new { message = "Company not found" });

                return Ok(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current company");
                return StatusCode(500, new { message = "An error occurred while retrieving your company" });
            }
        }

        [HttpPost]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<ActionResult<CompanyResponseDto>> CreateCompany([FromBody] CreateCompanyDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.CreatedById = _currentUserService.UserId ?? 
                    throw new UnauthorizedAccessException("UserId not found");

                var company = await _companyService.CreateCompanyAsync(dto);
                return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company");
                return StatusCode(500, new { message = "An error occurred while creating the company" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "CompanyAdmin")]
        public async Task<ActionResult<CompanyResponseDto>> UpdateCompany(Guid id, [FromBody] UpdateCompanyDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // SuperAdmin can update any company, others can only update their own
                if (_currentUserService.UserRole != "SuperAdmin" && id != _currentUserService.CompanyId)
                    return Forbid();

                dto.Id = id;
                dto.UpdatedById = _currentUserService.UserId ?? 
                    throw new UnauthorizedAccessException("UserId not found");

                var company = await _companyService.UpdateCompanyAsync(id, dto);
                if (company == null)
                    return NotFound(new { message = "Company not found" });

                return Ok(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company {CompanyId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the company" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            try
            {
                var result = await _companyService.DeleteCompanyAsync(id);
                if (!result)
                    return NotFound(new { message = "Company not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company {CompanyId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the company" });
            }
        }

        [HttpGet("{id}/statistics")]
        [Authorize(Policy = "CompanyAdmin")]
        public async Task<ActionResult<CompanyStatisticsDto>> GetCompanyStatistics(Guid id)
        {
            try
            {
                // SuperAdmin can view any company stats, others can only view their own
                if (_currentUserService.UserRole != "SuperAdmin" && id != _currentUserService.CompanyId)
                    return Forbid();

                var statistics = await _companyService.GetCompanyStatisticsAsync(id);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company statistics {CompanyId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving company statistics" });
            }
        }

        [HttpPut("{id}/subscription")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<ActionResult<CompanyResponseDto>> UpdateSubscription(
            Guid id, [FromBody] UpdateSubscriptionDto dto)
        {
            try
            {
                dto.CompanyId = id;
                dto.UpdatedById = _currentUserService.UserId ?? 
                    throw new UnauthorizedAccessException("UserId not found");

                var company = await _companyService.UpdateSubscriptionAsync(dto);
                if (company == null)
                    return NotFound(new { message = "Company not found" });

                return Ok(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company subscription {CompanyId}", id);
                return StatusCode(500, new { message = "An error occurred while updating subscription" });
            }
        }
    }
}