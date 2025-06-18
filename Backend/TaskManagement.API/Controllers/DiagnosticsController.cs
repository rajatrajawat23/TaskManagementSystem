using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class DiagnosticsController : ControllerBase
    {
        private readonly TaskManagementDbContext _context;
        private readonly ILogger<DiagnosticsController> _logger;

        public DiagnosticsController(TaskManagementDbContext context, ILogger<DiagnosticsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("test-task-creation")]
        public async Task<IActionResult> TestTaskCreation()
        {
            try
            {
                // Test minimal task creation
                var task = new Core.Entities.Task
                {
                    Title = "Diagnostic Test Task",
                    CompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    AssignedById = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CreatedById = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    UpdatedById = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    TaskNumber = $"TSK-TEST-{DateTime.UtcNow.Ticks}"
                };

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, taskId = task.Id, message = "Task created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test task creation");
                return StatusCode(500, new { 
                    error = ex.Message, 
                    innerError = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace 
                });
            }
        }

        [HttpGet("check-tables")]
        public async Task<IActionResult> CheckTables()
        {
            try
            {
                var tables = new Dictionary<string, object>();

                // Check TaskComments table
                var taskCommentsColumns = await _context.Database.SqlQueryRaw<TableColumn>(
                    @"SELECT COLUMN_NAME as ColumnName, DATA_TYPE as DataType, IS_NULLABLE as IsNullable 
                      FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'TaskComments' AND TABLE_SCHEMA = 'Core'
                      ORDER BY ORDINAL_POSITION").ToListAsync();

                tables["TaskComments"] = taskCommentsColumns;

                // Check SubTasks table
                var subTasksColumns = await _context.Database.SqlQueryRaw<TableColumn>(
                    @"SELECT COLUMN_NAME as ColumnName, DATA_TYPE as DataType, IS_NULLABLE as IsNullable 
                      FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'SubTasks' AND TABLE_SCHEMA = 'Core'
                      ORDER BY ORDINAL_POSITION").ToListAsync();

                tables["SubTasks"] = subTasksColumns;

                // Check Tasks table
                var tasksColumns = await _context.Database.SqlQueryRaw<TableColumn>(
                    @"SELECT COLUMN_NAME as ColumnName, DATA_TYPE as DataType, IS_NULLABLE as IsNullable 
                      FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'Tasks' AND TABLE_SCHEMA = 'Core'
                      ORDER BY ORDINAL_POSITION").ToListAsync();

                tables["Tasks"] = tasksColumns;

                return Ok(tables);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking tables");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("test-statistics")]
        public async Task<IActionResult> TestStatistics()
        {
            try
            {
                // Test the problematic query
                var tasks = await _context.Tasks
                    .Where(t => t.CompanyId == Guid.Parse("11111111-1111-1111-1111-111111111111") && !t.IsArchived)
                    .ToListAsync();

                // Now test the assignment query
                var taskAssignments = await _context.Tasks
                    .Where(t => t.CompanyId == Guid.Parse("11111111-1111-1111-1111-111111111111") && !t.IsArchived && t.AssignedToId != null)
                    .Include(t => t.AssignedTo)
                    .Select(t => new { 
                        t.AssignedToId, 
                        Name = t.AssignedTo != null ? t.AssignedTo.FirstName + " " + t.AssignedTo.LastName : "Unknown"
                    })
                    .ToListAsync();

                var result = new
                {
                    TotalTasks = tasks.Count,
                    TasksRetrieved = true,
                    AssignmentsRetrieved = true,
                    AssignmentCount = taskAssignments.Count
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test statistics");
                return StatusCode(500, new { 
                    error = ex.Message, 
                    innerError = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace 
                });
            }
        }

        [HttpGet("test-client-creation")]
        public async Task<IActionResult> TestClientCreation()
        {
            try
            {
                // Test minimal client creation
                var client = new Core.Entities.Client
                {
                    Name = "Diagnostic Test Client",
                    Email = "diagnostic@test.com",
                    CompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CreatedById = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    UpdatedById = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    IsActive = true
                };

                _context.Clients.Add(client);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, clientId = client.Id, message = "Client created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test client creation");
                return StatusCode(500, new { 
                    error = ex.Message, 
                    innerError = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace 
                });
            }
        }

        private class TableColumn
        {
            public string ColumnName { get; set; }
            public string DataType { get; set; }
            public string IsNullable { get; set; }
        }
    }
}
