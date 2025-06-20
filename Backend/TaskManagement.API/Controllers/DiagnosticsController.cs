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
                    TaskNumber = "TSK-DIAG-001"
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

        [HttpGet("check-task-triggers")]
        public async Task<IActionResult> CheckTaskTriggers()
        {
            try
            {
                // First check for triggers (this might return empty but shouldn't cause null errors)
                List<TriggerInfo> triggers = new List<TriggerInfo>();
                try
                {
                    triggers = await _context.Database.SqlQueryRaw<TriggerInfo>(
                        @"SELECT 
                            ISNULL(t.name, '') AS TriggerName,
                            ISNULL(OBJECT_NAME(t.parent_id), '') AS TableName,
                            ISNULL(m.definition, '') AS TriggerDefinition
                        FROM sys.triggers t
                        INNER JOIN sys.sql_modules m ON t.object_id = m.object_id
                        WHERE OBJECT_NAME(t.parent_id) = 'Tasks'").ToListAsync();
                }
                catch (Exception triggerEx)
                {
                    _logger.LogWarning(triggerEx, "Could not retrieve trigger information");
                }

                // Check column information with null-safe query
                List<ColumnInfo> columns = new List<ColumnInfo>();
                try
                {
                    columns = await _context.Database.SqlQueryRaw<ColumnInfo>(
                        @"SELECT 
                            ISNULL(COLUMN_NAME, '') as ColumnName,
                            ISNULL(DATA_TYPE, '') as DataType,
                            CHARACTER_MAXIMUM_LENGTH as MaxLength,
                            ISNULL(COLUMN_DEFAULT, '') as DefaultValue
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'Tasks' AND TABLE_SCHEMA = 'Core' AND COLUMN_NAME = 'TaskNumber'").ToListAsync();
                }
                catch (Exception columnEx)
                {
                    _logger.LogWarning(columnEx, "Could not retrieve column information");
                }

                // Handle empty results gracefully
                var result = new
                {
                    success = true,
                    message = triggers?.Count > 0 ? $"Found {triggers.Count} trigger(s) on Tasks table" : "No triggers found on Tasks table",
                    triggerCount = triggers?.Count ?? 0,
                    triggers = triggers ?? new List<TriggerInfo>(),
                    taskNumberColumn = columns?.FirstOrDefault(),
                    columnCount = columns?.Count ?? 0,
                    hasTaskNumberColumn = columns?.Any() == true,
                    databaseStatus = "Connected and accessible"
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking task triggers");
                return StatusCode(500, new { 
                    success = false,
                    error = ex.Message,
                    message = "Failed to check task triggers"
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

        private class TriggerInfo
        {
            public string TriggerName { get; set; } = string.Empty;
            public string TableName { get; set; } = string.Empty;
            public string TriggerDefinition { get; set; } = string.Empty;
        }

        private class ColumnInfo
        {
            public string ColumnName { get; set; } = string.Empty;
            public string DataType { get; set; } = string.Empty;
            public int? MaxLength { get; set; }
            public string DefaultValue { get; set; } = string.Empty;
        }
    }
}
