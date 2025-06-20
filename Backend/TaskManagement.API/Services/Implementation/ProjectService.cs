using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.API.Services.Interfaces;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Services.Implementation
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProjectService> _logger;
        private readonly ICurrentUserService _currentUserService;

        public ProjectService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProjectService> logger,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<ProjectResponseDto>> GetProjectsAsync(
            Guid companyId, int pageNumber, int pageSize, string? search, 
            string? status, Guid? clientId, Guid? projectManagerId, 
            string? sortBy, bool sortDescending)
        {
            try
            {
                var currentCompanyId = _currentUserService.CompanyId;
                if (currentCompanyId == null && _currentUserService.UserRole != "SuperAdmin")
                    throw new UnauthorizedAccessException("CompanyId not found");

                IQueryable<Project> query = _unitOfWork.Projects.Query()
                    .Include(p => p.ProjectManager)
                    .Include(p => p.Client)
                    .Include(p => p.Tasks);

                // Filter by company only if not SuperAdmin
                if (_currentUserService.UserRole != "SuperAdmin")
                {
                    query = query.Where(p => p.CompanyId == companyId);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p => p.Name.Contains(search) ||
                                           p.Description.Contains(search) ||
                                           p.ProjectCode.Contains(search));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(p => p.Status == status);
                }

                if (clientId.HasValue)
                {
                    query = query.Where(p => p.ClientId == clientId.Value);
                }

                if (projectManagerId.HasValue)
                {
                    query = query.Where(p => p.ProjectManagerId == projectManagerId.Value);
                }

                // Apply sorting
                query = sortBy?.ToLower() switch
                {
                    "name" => sortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                    "projectcode" => sortDescending ? query.OrderByDescending(p => p.ProjectCode) : query.OrderBy(p => p.ProjectCode),
                    "status" => sortDescending ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status),
                    "startdate" => sortDescending ? query.OrderByDescending(p => p.StartDate) : query.OrderBy(p => p.StartDate),
                    "createdat" => sortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                    _ => sortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
                };

                var totalCount = await query.CountAsync();

                var projects = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var projectDtos = _mapper.Map<List<ProjectResponseDto>>(projects);

                // Populate team members for all projects
                await PopulateTeamMembersAsync(projects, projectDtos);

                return new PagedResult<ProjectResponseDto>
                {
                    Items = projectDtos,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all projects");
                throw;
            }
        }

        public async Task<ProjectResponseDto?> GetProjectByIdAsync(Guid projectId, Guid companyId)
        {
            try
            {
                var currentCompanyId = _currentUserService.CompanyId;
                if (currentCompanyId == null && _currentUserService.UserRole != "SuperAdmin")
                    throw new UnauthorizedAccessException("CompanyId not found");

                var query = _unitOfWork.Projects.Query()
                    .Include(p => p.ProjectManager)
                    .Include(p => p.Client)
                    .Include(p => p.Tasks)
                    .Where(p => p.Id == projectId);

                // Filter by company only if not SuperAdmin
                if (_currentUserService.UserRole != "SuperAdmin")
                {
                    query = query.Where(p => p.CompanyId == companyId);
                }

                var project = await query.FirstOrDefaultAsync();

                if (project == null) return null;

                var projectDto = _mapper.Map<ProjectResponseDto>(project);

                // Populate team members
                if (!string.IsNullOrEmpty(project.TeamMembers))
                {
                    var teamMemberIds = JsonSerializer.Deserialize<List<Guid>>(project.TeamMembers);
                    if (teamMemberIds != null && teamMemberIds.Any())
                    {
                        var teamMembers = await _unitOfWork.Users.Query()
                            .Where(u => teamMemberIds.Contains(u.Id))
                            .Select(u => new TeamMemberDto
                            {
                                UserId = u.Id,
                                Name = $"{u.FirstName} {u.LastName}",
                                Email = u.Email,
                                Role = u.Role,
                                ProfileImageUrl = u.ProfileImageUrl
                            })
                            .ToListAsync();

                        projectDto.TeamMembers = teamMembers;
                    }
                }

                return projectDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project by ID: {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto)
        {
            try
            {
                _logger.LogInformation("Starting project creation for {ProjectName}", createProjectDto.Name);
                
                var companyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("UserId not found");

                _logger.LogInformation("CompanyId: {CompanyId}, UserId: {UserId}", companyId, userId);

                var project = _mapper.Map<Project>(createProjectDto);
                _logger.LogInformation("AutoMapper mapping completed");
                
                project.CompanyId = companyId;
                
                _logger.LogInformation("Generating project code...");
                project.ProjectCode = await _unitOfWork.Projects.GenerateProjectCodeAsync(companyId);
                _logger.LogInformation("Project code generated: {ProjectCode}", project.ProjectCode);
                
                project.CreatedById = userId;
                project.UpdatedById = userId;
                project.ProjectManagerId = createProjectDto.ManagerId;

                _logger.LogInformation("Project mapped, ManagerId: {ManagerId}", project.ProjectManagerId);

                // Convert team members list to JSON
                if (createProjectDto.TeamMemberIds != null && createProjectDto.TeamMemberIds.Any())
                {
                    project.TeamMembers = JsonSerializer.Serialize(createProjectDto.TeamMemberIds);
                    _logger.LogInformation("Team members serialized: {TeamMembers}", project.TeamMembers);
                }

                _logger.LogInformation("Adding project to repository...");
                await _unitOfWork.Projects.AddAsync(project);
                
                _logger.LogInformation("Saving changes to database...");
                await _unitOfWork.SaveChangesAsync();
                
                _logger.LogInformation("Project created successfully with ID: {ProjectId}", project.Id);

                return await GetProjectByIdAsync(project.Id, project.CompanyId) ?? throw new InvalidOperationException("Failed to get created project");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project: {Message}", ex.Message);
                _logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);
                throw;
            }
        }

        public async Task<ProjectResponseDto?> UpdateProjectAsync(Guid projectId, UpdateProjectDto updateProjectDto)
        {
            try
            {
                var companyId = _currentUserService.CompanyId;
                var userId = _currentUserService.UserId;

                var project = await _unitOfWork.Projects.Query()
                    .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

                if (project == null)
                    throw new KeyNotFoundException($"Project with ID {projectId} not found");

                _mapper.Map(updateProjectDto, project);
                project.ProjectManagerId = updateProjectDto.ManagerId;
                project.UpdatedById = userId;
                project.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Projects.Update(project);
                await _unitOfWork.SaveChangesAsync();

                return await GetProjectByIdAsync(project.Id, project.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project: {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<bool> DeleteProjectAsync(Guid projectId, Guid companyId)
        {
            try
            {
                var currentCompanyId = _currentUserService.CompanyId;
                if (currentCompanyId == null && _currentUserService.UserRole != "SuperAdmin")
                    throw new UnauthorizedAccessException("CompanyId not found");

                var query = _unitOfWork.Projects.Query().Where(p => p.Id == projectId);

                // Filter by company only if not SuperAdmin
                if (_currentUserService.UserRole != "SuperAdmin")
                {
                    query = query.Where(p => p.CompanyId == companyId);
                }

                var project = await query.FirstOrDefaultAsync();

                if (project == null)
                    return false;

                project.IsDeleted = true;
                project.UpdatedAt = DateTime.UtcNow;
                project.UpdatedById = _currentUserService.UserId;

                _unitOfWork.Projects.Update(project);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project: {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<IEnumerable<ProjectResponseDto>> GetProjectsByClientAsync(Guid clientId)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                var projects = await _unitOfWork.Projects.GetProjectsByClientAsync(clientId, companyId);
                return _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects by client: {ClientId}", clientId);
                throw;
            }
        }

        public async Task<IEnumerable<ProjectResponseDto>> GetProjectsByManagerAsync(Guid managerId)
        {
            try
            {
                var companyId = _currentUserService.CompanyId ?? throw new UnauthorizedAccessException("CompanyId not found");
                var projects = await _unitOfWork.Projects.GetProjectsByManagerAsync(managerId, companyId);
                return _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects by manager: {ManagerId}", managerId);
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetProjectStatisticsAsync(Guid companyId)
        {
            try
            {
                return await _unitOfWork.Projects.GetProjectStatisticsByStatusAsync(companyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project statistics");
                throw;
            }
        }

        public async Task<ProjectResponseDto?> AddTeamMemberAsync(AddTeamMemberDto dto)
        {
            try
            {
                var currentCompanyId = _currentUserService.CompanyId;
                if (currentCompanyId == null && _currentUserService.UserRole != "SuperAdmin")
                    throw new UnauthorizedAccessException("CompanyId not found");

                var query = _unitOfWork.Projects.Query().Where(p => p.Id == dto.ProjectId);

                // Filter by company only if not SuperAdmin
                if (_currentUserService.UserRole != "SuperAdmin")
                {
                    query = query.Where(p => p.CompanyId == dto.CompanyId);
                }

                var project = await query.FirstOrDefaultAsync();

                if (project == null)
                    return null;

                // Parse existing team members
                var teamMembers = string.IsNullOrEmpty(project.TeamMembers) 
                    ? new List<Guid>() 
                    : JsonSerializer.Deserialize<List<Guid>>(project.TeamMembers) ?? new List<Guid>();

                if (!teamMembers.Contains(dto.UserId))
                {
                    teamMembers.Add(dto.UserId);
                    project.TeamMembers = JsonSerializer.Serialize(teamMembers);
                    project.UpdatedAt = DateTime.UtcNow;
                    project.UpdatedById = _currentUserService.UserId;

                    _unitOfWork.Projects.Update(project);
                    await _unitOfWork.SaveChangesAsync();
                }

                return await GetProjectByIdAsync(project.Id, project.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding team member to project: {ProjectId}", dto.ProjectId);
                throw;
            }
        }

        public async Task<ProjectResponseDto?> RemoveTeamMemberAsync(Guid projectId, Guid userId, Guid companyId, Guid updatedById)
        {
            try
            {
                var currentCompanyId = _currentUserService.CompanyId;
                if (currentCompanyId == null && _currentUserService.UserRole != "SuperAdmin")
                    throw new UnauthorizedAccessException("CompanyId not found");

                var query = _unitOfWork.Projects.Query().Where(p => p.Id == projectId);

                // Filter by company only if not SuperAdmin
                if (_currentUserService.UserRole != "SuperAdmin")
                {
                    query = query.Where(p => p.CompanyId == companyId);
                }

                var project = await query.FirstOrDefaultAsync();

                if (project == null)
                    return null;

                // Parse existing team members
                var teamMembers = string.IsNullOrEmpty(project.TeamMembers)
                    ? new List<Guid>()
                    : JsonSerializer.Deserialize<List<Guid>>(project.TeamMembers) ?? new List<Guid>();

                if (teamMembers.Contains(userId))
                {
                    teamMembers.Remove(userId);
                    project.TeamMembers = JsonSerializer.Serialize(teamMembers);
                    project.UpdatedAt = DateTime.UtcNow;
                    project.UpdatedById = updatedById;

                    _unitOfWork.Projects.Update(project);
                    await _unitOfWork.SaveChangesAsync();
                }

                return await GetProjectByIdAsync(project.Id, project.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing team member from project: {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<ProjectResponseDto> UpdateProjectStatusAsync(Guid projectId, string status)
        {
            try
            {
                var companyId = _currentUserService.CompanyId;
                var project = await _unitOfWork.Projects.Query()
                    .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

                if (project == null)
                    throw new KeyNotFoundException($"Project with ID {projectId} not found");

                project.Status = status;
                project.UpdatedAt = DateTime.UtcNow;
                project.UpdatedById = _currentUserService.UserId;

                _unitOfWork.Projects.Update(project);
                await _unitOfWork.SaveChangesAsync();

                return await GetProjectByIdAsync(project.Id, project.CompanyId) ?? throw new InvalidOperationException("Failed to get updated project");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project status: {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<IEnumerable<TaskResponseDto>> GetProjectTasksAsync(Guid projectId, Guid companyId)
        {
            try
            {
                var currentCompanyId = _currentUserService.CompanyId;
                if (currentCompanyId == null && _currentUserService.UserRole != "SuperAdmin")
                    throw new UnauthorizedAccessException("CompanyId not found");
                var query = _unitOfWork.Tasks.Query()
                    .Include(t => t.AssignedTo)
                    .Include(t => t.AssignedBy)
                    .Where(t => t.ProjectId == projectId);

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
                _logger.LogError(ex, "Error getting project tasks: {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<ProjectStatisticsDto> GetProjectStatisticsAsync(Guid projectId, Guid companyId)
        {
            try
            {
                var currentCompanyId = _currentUserService.CompanyId;
                if (currentCompanyId == null && _currentUserService.UserRole != "SuperAdmin")
                    throw new UnauthorizedAccessException("CompanyId not found");

                var query = _unitOfWork.Projects.Query()
                    .Include(p => p.Tasks)
                    .Where(p => p.Id == projectId);

                // Filter by company only if not SuperAdmin
                if (_currentUserService.UserRole != "SuperAdmin")
                {
                    query = query.Where(p => p.CompanyId == companyId);
                }

                var project = await query.FirstOrDefaultAsync();

                if (project == null)
                    throw new KeyNotFoundException($"Project with ID {projectId} not found");

                var tasks = project.Tasks.Where(t => !t.IsDeleted).ToList();

                var statistics = new ProjectStatisticsDto
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    TotalTasks = tasks.Count,
                    CompletedTasks = tasks.Count(t => t.Status == "Completed"),
                    InProgressTasks = tasks.Count(t => t.Status == "InProgress"),
                    PendingTasks = tasks.Count(t => t.Status == "Pending"),
                    OverdueTasks = tasks.Count(t => t.Status != "Completed" && t.DueDate < DateTime.UtcNow),
                    BudgetUtilization = project.Budget.HasValue && project.Budget > 0 
                        ? CalculateBudgetUtilization(project, tasks) 
                        : null,
                    AverageTaskCompletionTime = CalculateAverageCompletionTime(tasks),
                    TasksByPriority = tasks.GroupBy(t => t.Priority ?? "Medium")
                        .ToDictionary(g => g.Key, g => g.Count()),
                    HoursByTeamMember = await CalculateHoursByTeamMemberAsync(project.Id),
                    Milestones = await GetProjectMilestonesAsync(project.Id)
                };

                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project statistics: {ProjectId}", projectId);
                throw;
            }
        }

        private decimal? CalculateBudgetUtilization(Project project, List<Core.Entities.Task> tasks)
        {
            // Calculate based on estimated hours * hourly rate or some other logic
            // For now, we'll use a simple percentage based on completed tasks
            if (!project.Budget.HasValue || project.Budget <= 0)
                return null;

            var totalEstimatedHours = tasks.Sum(t => t.EstimatedHours ?? 0);
            var totalActualHours = tasks.Sum(t => t.ActualHours ?? 0);
            
            // Assume $100/hour as a default rate - in real app this would come from configuration
            var estimatedCost = totalActualHours * 100;
            
            return estimatedCost / project.Budget.Value * 100;
        }

        private double CalculateAverageCompletionTime(List<Core.Entities.Task> tasks)
        {
            var completedTasks = tasks.Where(t => t.Status == "Completed" && t.CompletedDate.HasValue).ToList();
            if (!completedTasks.Any())
                return 0;

            var totalDays = completedTasks.Sum(t => (t.CompletedDate!.Value - t.CreatedAt).TotalDays);
            return totalDays / completedTasks.Count;
        }

        private async Task<Dictionary<string, decimal>> CalculateHoursByTeamMemberAsync(Guid projectId)
        {
            var tasks = await _unitOfWork.Tasks.Query()
                .Include(t => t.AssignedTo)
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .ToListAsync();

            return tasks.Where(t => t.AssignedTo != null)
                .GroupBy(t => $"{t.AssignedTo!.FirstName} {t.AssignedTo.LastName}")
                .ToDictionary(g => g.Key, g => g.Sum(t => t.ActualHours ?? 0));
        }

        private async Task<List<ProjectMilestoneDto>> GetProjectMilestonesAsync(Guid projectId)
        {
            // This is a placeholder - in a real system, you might have a separate Milestones table
            // For now, we'll create milestones based on task groups or phases
            var tasks = await _unitOfWork.Tasks.Query()
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .ToListAsync();

            var milestones = new List<ProjectMilestoneDto>();

            // Example: Create milestones based on task categories or phases
            var taskGroups = tasks.GroupBy(t => t.Category ?? "General").ToList();
            
            foreach (var group in taskGroups)
            {
                var groupTasks = group.ToList();
                if (groupTasks.Any())
                {
                    milestones.Add(new ProjectMilestoneDto
                    {
                        Name = group.Key,
                        DueDate = groupTasks.Max(t => t.DueDate ?? DateTime.UtcNow.AddDays(30)),
                        IsCompleted = groupTasks.All(t => t.Status == "Completed"),
                        CompletionPercentage = (int)(groupTasks.Count(t => t.Status == "Completed") * 100.0 / groupTasks.Count)
                    });
                }
            }

            return milestones.OrderBy(m => m.DueDate).ToList();
        }

        private async System.Threading.Tasks.Task PopulateTeamMembersAsync(List<Project> projects, List<ProjectResponseDto> projectDtos)
        {
            // Get all unique team member IDs from all projects
            var allTeamMemberIds = new HashSet<Guid>();
            
            foreach (var project in projects)
            {
                if (!string.IsNullOrEmpty(project.TeamMembers))
                {
                    var teamMemberIds = JsonSerializer.Deserialize<List<Guid>>(project.TeamMembers);
                    if (teamMemberIds != null)
                    {
                        foreach (var id in teamMemberIds)
                        {
                            allTeamMemberIds.Add(id);
                        }
                    }
                }
            }

            if (!allTeamMemberIds.Any()) return;

            // Load all team members in a single query
            var teamMembersDict = await _unitOfWork.Users.Query()
                .Where(u => allTeamMemberIds.Contains(u.Id))
                .Select(u => new TeamMemberDto
                {
                    UserId = u.Id,
                    Name = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    Role = u.Role,
                    ProfileImageUrl = u.ProfileImageUrl
                })
                .ToDictionaryAsync(tm => tm.UserId);

            // Populate team members for each project
            for (int i = 0; i < projects.Count; i++)
            {
                if (!string.IsNullOrEmpty(projects[i].TeamMembers))
                {
                    var teamMemberIds = JsonSerializer.Deserialize<List<Guid>>(projects[i].TeamMembers);
                    if (teamMemberIds != null)
                    {
                        projectDtos[i].TeamMembers = teamMemberIds
                            .Where(id => teamMembersDict.ContainsKey(id))
                            .Select(id => teamMembersDict[id])
                            .ToList();
                    }
                }
            }
        }
    }
}