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
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(
            IProjectService projectService,
            ICurrentUserService currentUserService,
            ILogger<ProjectController> logger)
        {
            _projectService = projectService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ProjectResponseDto>>> GetProjects(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] Guid? clientId = null,
            [FromQuery] Guid? projectManagerId = null,
            [FromQuery] string? sortBy = "CreatedAt",
            [FromQuery] bool sortDescending = true)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");
                
                var result = await _projectService.GetProjectsAsync(
                    companyId, pageNumber, pageSize, search, status, 
                    clientId, projectManagerId, sortBy, sortDescending);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects");
                return StatusCode(500, new { message = "An error occurred while retrieving projects" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponseDto>> GetProject(Guid id)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");
                
                var project = await _projectService.GetProjectByIdAsync(id, companyId);
                if (project == null)
                    return NotFound(new { message = "Project not found" });

                return Ok(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project {ProjectId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the project" });
            }
        }

        [HttpPost]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult<ProjectResponseDto>> CreateProject([FromBody] CreateProjectDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.CompanyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");
                dto.CreatedById = _currentUserService.UserId ?? 
                    throw new UnauthorizedAccessException("UserId not found");

                var project = await _projectService.CreateProjectAsync(dto);
                return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                return StatusCode(500, new { message = "An error occurred while creating the project" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult<ProjectResponseDto>> UpdateProject(Guid id, [FromBody] UpdateProjectDto dto)
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

                var project = await _projectService.UpdateProjectAsync(id, dto);
                if (project == null)
                    return NotFound(new { message = "Project not found" });

                return Ok(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project {ProjectId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the project" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");
                
                var result = await _projectService.DeleteProjectAsync(id, companyId);
                if (!result)
                    return NotFound(new { message = "Project not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project {ProjectId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the project" });
            }
        }

        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetProjectTasks(Guid id)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");
                
                var tasks = await _projectService.GetProjectTasksAsync(id, companyId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for project {ProjectId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving project tasks" });
            }
        }

        [HttpPost("{id}/team-member")]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult<ProjectResponseDto>> AddTeamMember(Guid id, [FromBody] AddTeamMemberDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.ProjectId = id;
                dto.CompanyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");

                var project = await _projectService.AddTeamMemberAsync(dto);
                if (project == null)
                    return NotFound(new { message = "Project not found" });

                return Ok(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding team member to project {ProjectId}", id);
                return StatusCode(500, new { message = "An error occurred while adding team member" });
            }
        }

        [HttpDelete("{id}/team-member/{userId}")]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult<ProjectResponseDto>> RemoveTeamMember(Guid id, Guid userId)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");
                var updatedById = _currentUserService.UserId ?? 
                    throw new UnauthorizedAccessException("UserId not found");

                var project = await _projectService.RemoveTeamMemberAsync(id, userId, companyId, updatedById);
                if (project == null)
                    return NotFound(new { message = "Project not found" });

                return Ok(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing team member from project {ProjectId}", id);
                return StatusCode(500, new { message = "An error occurred while removing team member" });
            }
        }

        [HttpGet("{id}/statistics")]
        public async Task<ActionResult<ProjectStatisticsDto>> GetProjectStatistics(Guid id)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? 
                    throw new UnauthorizedAccessException("CompanyId not found");
                
                var statistics = await _projectService.GetProjectStatisticsAsync(id, companyId);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics for project {ProjectId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving project statistics" });
            }
        }
    }
}