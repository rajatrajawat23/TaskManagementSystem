# Background Services Documentation

## Overview

The Task Management System includes four background services that run automatically to handle various maintenance and notification tasks. These services operate independently of user requests and ensure the system remains efficient and users stay informed.

## 1. Email Notification Service

**Purpose**: Sends automated email notifications for task reminders and overdue alerts.

**Schedule**: Runs every 30 minutes

**Features**:
- **Task Reminders**: Sends email notifications 24 hours before task due dates
- **Overdue Alerts**: Notifies both assignee and assigner when tasks become overdue
- **Smart Filtering**: Only sends notifications for active tasks and active users

**Key Operations**:
```csharp
// Reminder emails for tasks due tomorrow
await ProcessTaskReminders();

// Overdue notifications for tasks that missed their deadline
await ProcessOverdueTasks();
```

**Configuration**:
- Check interval: 30 minutes
- Reminder period: 24 hours before due date
- Overdue check: Tasks overdue within last 24 hours

## 2. Recurring Task Service

**Purpose**: Automatically creates new instances of recurring tasks based on their defined patterns.

**Schedule**: Runs every hour

**Features**:
- **Pattern Support**: Daily, Weekly, Monthly, and Yearly recurrence
- **Flexible Configuration**: Supports interval-based recurrence (e.g., every 3 days)
- **Smart Instance Creation**: Only creates new instances when due
- **Parent-Child Relationship**: Links recurring instances to their parent task

**Supported Recurrence Patterns**:
```json
{
  "Type": "Weekly",        // Daily, Weekly, Monthly, Yearly
  "Interval": 1,           // Every N days/weeks/months/years
  "DaysOfWeek": [1, 3, 5], // For weekly: Monday, Wednesday, Friday
  "DayOfMonth": 15,        // For monthly: specific day
  "EndDate": "2025-12-31", // Optional end date
  "MaxOccurrences": 52     // Optional max instances
}
```

**Key Operations**:
- Scans all active recurring tasks
- Calculates next due dates based on patterns
- Creates new task instances with appropriate dates
- Maintains task numbering sequence

## 3. Data Cleanup Service

**Purpose**: Maintains database performance by archiving old data and cleaning up orphaned records.

**Schedule**: Runs daily (with 5-minute startup delay)

**Features**:
- **Task Archival**: Archives completed tasks older than 6 months
- **Notification Cleanup**: Removes read notifications older than 30 days
- **Orphaned Attachments**: Identifies and removes attachments for deleted tasks
- **Activity Log Cleanup**: Removes activity logs older than 3 months
- **Soft Delete Purge**: Permanently removes soft-deleted records after 90 days

**Retention Policies**:
| Data Type | Retention Period | Action |
|-----------|-----------------|--------|
| Completed Tasks | 6 months | Archive |
| Read Notifications | 30 days | Delete |
| Activity Logs | 3 months | Delete |
| Soft-Deleted Records | 90 days | Permanent Delete |

**Safety Features**:
- Checks for dependencies before purging
- Logs all cleanup operations
- Graceful error handling per operation

## 4. Report Generation Service

**Purpose**: Generates and emails weekly performance reports to company administrators.

**Schedule**: Runs weekly (with 10-minute startup delay)

**Features**:
- **Company-Specific Reports**: Generates separate reports for each active company
- **Comprehensive Metrics**: Tasks created/completed, overdue items, active projects
- **User Performance**: Top 5 performers based on task completion
- **Upcoming Deadlines**: Next 7 days of due tasks
- **HTML Email Format**: Professional, formatted reports

**Report Contents**:
1. **Summary Section**:
   - Tasks created this week
   - Tasks completed this week
   - Currently overdue tasks
   - Active projects count
   - Active users count

2. **Performance Section**:
   - Top 5 users by task completion
   - Completion counts per user

3. **Planning Section**:
   - Tasks due in next 7 days
   - Priority indicators
   - Assignee information

**Email Recipients**: All Company Admins for each company

## Configuration

All background services are registered in `ServiceExtensions.cs`:

```csharp
// Add Background Services
services.AddHostedService<EmailNotificationService>();
services.AddHostedService<RecurringTaskService>();
services.AddHostedService<DataCleanupService>();
services.AddHostedService<ReportGenerationService>();
```

## Monitoring

Background service activity is logged through Serilog:
- Service start/stop events
- Operation counts and results
- Error details with stack traces
- Performance metrics

**Log Examples**:
```
[INF] Email Notification Service started
[INF] Reminder sent for task TSK-2024-0001 to user@example.com
[INF] Archived 45 old completed tasks
[INF] Weekly report sent to admin@company.com for Tech Corp
```

## Error Handling

All services implement robust error handling:
- **Isolated Operations**: Errors in one operation don't affect others
- **Automatic Retry**: Services wait and retry after errors
- **Graceful Degradation**: Services continue running even if some operations fail
- **Detailed Logging**: All errors are logged with context

## Performance Considerations

1. **Staggered Startup**: Services have different startup delays to avoid simultaneous load
2. **Efficient Queries**: All database queries use proper filtering and includes
3. **Batch Operations**: Cleanup and archival done in batches
4. **Cancellation Support**: All services properly handle application shutdown

## Future Enhancements

Potential improvements for background services:
1. **Configurable Schedules**: Make intervals configurable via settings
2. **Manual Triggers**: Admin endpoints to manually run services
3. **Service Health Dashboard**: Real-time monitoring of service status
4. **Custom Report Schedules**: Allow companies to choose report frequency
5. **Webhook Integration**: Send notifications via webhooks
6. **Advanced Recurrence**: Support for complex patterns (e.g., last Friday of month)

## Testing Background Services

To test background services in development:

1. **Reduce Intervals**: Temporarily change check intervals for faster testing
2. **Add Test Data**: Create tasks with appropriate dates
3. **Check Logs**: Monitor service logs for operation results
4. **Database Verification**: Verify changes in database after operations

Example test scenario:
```csharp
// Create a task due tomorrow for reminder testing
var testTask = new Task
{
    Title = "Test Reminder Task",
    DueDate = DateTime.UtcNow.AddDays(1),
    Status = "Pending",
    // ... other properties
};
```

## Deployment Notes

For production deployment:
1. Ensure email settings are configured in `appsettings.json`
2. Consider using distributed locks for multi-instance deployments
3. Monitor service logs for performance issues
4. Adjust intervals based on system load
5. Implement alerting for service failures

---

These background services ensure the Task Management System operates efficiently and keeps users informed without manual intervention. They form a critical part of the system's automation capabilities.
