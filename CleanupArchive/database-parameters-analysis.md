# 📊 Database Parameters Analysis - TaskManagementSystem

## Overview
This document details exactly what parameters each service sends to the database for CRUD operations.

---

## 🔧 TaskService Database Operations

### 1. **CREATE Task** (`CreateTaskAsync`)
```csharp
// Entity being created
var task = new Task
{
    Title = request.Title,
    Description = request.Description,
    TaskNumber = "TSK-2025-0001", // Auto-generated
    Priority = request.Priority,
    Status = request.Status ?? "Pending",
    Category = request.Category,
    EstimatedHours = request.EstimatedHours,
    StartDate = request.StartDate,
    DueDate = request.DueDate,
    AssignedToId = request.AssignedToId,
    AssignedById = currentUserId,
    ClientId = request.ClientId,
    ProjectId = request.ProjectId,
    CompanyId = request.CompanyId,
    CreatedById = currentUserId,
    Tags = request.Tags, // JSON string
    Progress = 0,
    IsRecurring = request.IsRecurring,
    RecurrencePattern = request.RecurrencePattern
}

// Database operation
await _unitOfWork.Tasks.AddAsync(task);
await _unitOfWork.SaveChangesAsync();
```

### 2. **READ Tasks** (`GetTasksAsync`)
```csharp
// Query with includes
IQueryable<Task> query = _unitOfWork.Tasks.Query()
    .Include(t => t.AssignedTo)
    .Include(t => t.AssignedBy)
    .Include(t => t.Client)
    .Include(t => t.Project);

// Filters applied
.Where(t => t.CompanyId == companyId)
.Where(t => t.Status == status)
.Where(t => t.Priority == priority)
.Where(t => t.AssignedToId == assignedToId)
.Where(t => t.StartDate >= startDate)
.Where(t => t.DueDate <= endDate)
.Where(t => t.Title.Contains(search) || t.Description.Contains(search))

// Pagination
.Skip((pageNumber - 1) * pageSize)
.Take(pageSize)
```

### 3. **UPDATE Task** (`UpdateTaskAsync`)
```csharp
// Fields being updated
task.Title = updateRequest.Title;
task.Description = updateRequest.Description;
task.Priority = updateRequest.Priority;
task.Status = updateRequest.Status;
task.Category = updateRequest.Category;
task.EstimatedHours = updateRequest.EstimatedHours;
task.ActualHours = updateRequest.ActualHours;
task.StartDate = updateRequest.StartDate;
task.DueDate = updateRequest.DueDate;
task.Progress = updateRequest.Progress;
task.UpdatedById = currentUserId;
task.UpdatedAt = DateTime.UtcNow;

// Special handling for status
if (status == "Completed") {
    task.CompletedDate = DateTime.UtcNow;
    task.Progress = 100;
}

_unitOfWork.Tasks.Update(task);
await _unitOfWork.SaveChangesAsync();
```

### 4. **DELETE Task** (Soft Delete)
```csharp
task.IsDeleted = true;
task.UpdatedById = currentUserId;
task.UpdatedAt = DateTime.UtcNow;

_unitOfWork.Tasks.Update(task);
await _unitOfWork.SaveChangesAsync();
```

---

## 👤 UserService Database Operations

### 1. **CREATE User** (`CreateUserAsync`)
```csharp
var user = new User
{
    Email = request.Email,
    PasswordHash = BCrypt.HashPassword(request.Password),
    FirstName = request.FirstName,
    LastName = request.LastName,
    PhoneNumber = request.PhoneNumber,
    Role = request.Role,
    Department = request.Department,
    JobTitle = request.JobTitle,
    CompanyId = request.CompanyId,
    IsActive = true,
    EmailVerified = false,
    CreatedById = currentUserId
}

await _unitOfWork.Users.AddAsync(user);
await _unitOfWork.SaveChangesAsync();
```

### 2. **READ Users** (`GetUsersAsync`)
```csharp
// Query with company filter
var query = _unitOfWork.Users.Query()
    .Where(u => u.CompanyId == companyId);

// Additional filters
.Where(u => u.Role == role)
.Where(u => u.IsActive == isActive)
.Where(u => u.FirstName.Contains(search) || 
            u.LastName.Contains(search) || 
            u.Email.Contains(search))

// Sorting
.OrderBy(u => u.FirstName)
.OrderByDescending(u => u.CreatedAt)
```

### 3. **UPDATE User** (`UpdateUserAsync`)
```csharp
user.FirstName = request.FirstName;
user.LastName = request.LastName;
user.PhoneNumber = request.PhoneNumber;
user.Department = request.Department;
user.JobTitle = request.JobTitle;
user.UpdatedById = currentUserId;
user.UpdatedAt = DateTime.UtcNow;

_unitOfWork.Users.Update(user);
await _unitOfWork.SaveChangesAsync();
```

---

## 🏢 ClientService Database Operations

### 1. **CREATE Client** (`CreateClientAsync`)
```csharp
var client = new Client
{
    Name = request.Name,
    Email = request.Email,
    Phone = request.Phone,
    Address = request.Address,
    ContactPerson = request.ContactPerson,
    Website = request.Website,
    CompanyId = request.CompanyId,
    IsActive = true,
    CreatedById = currentUserId
}

await _unitOfWork.Clients.AddAsync(client);
await _unitOfWork.SaveChangesAsync();
```

### 2. **READ Clients with Related Data**
```csharp
// Get client's projects
var projects = await _unitOfWork.Projects.Query()
    .Where(p => p.ClientId == clientId)
    .Where(p => p.CompanyId == companyId)
    .OrderByDescending(p => p.CreatedAt)
    .ToListAsync();

// Get client's tasks
var tasks = await _unitOfWork.Tasks.Query()
    .Where(t => t.ClientId == clientId)
    .Where(t => t.CompanyId == companyId)
    .Include(t => t.AssignedTo)
    .OrderByDescending(t => t.CreatedAt)
    .ToListAsync();
```

---

## 📁 ProjectService Database Operations

### 1. **CREATE Project** (`CreateProjectAsync`)
```csharp
var project = new Project
{
    Name = request.Name,
    Description = request.Description,
    ClientId = request.ClientId,
    Status = request.Status ?? "Planning",
    Budget = request.Budget,
    StartDate = request.StartDate,
    EndDate = request.EndDate,
    CompanyId = request.CompanyId,
    CreatedById = currentUserId
}

await _unitOfWork.Projects.AddAsync(project);
await _unitOfWork.SaveChangesAsync();
```

### 2. **READ Projects with Task Count**
```csharp
var query = _unitOfWork.Projects.Query()
    .Include(p => p.Client)
    .Include(p => p.Tasks)
    .Where(p => p.CompanyId == companyId);

// Project response with task statistics
var projects = await query.Select(p => new ProjectResponseDto
{
    Id = p.Id,
    Name = p.Name,
    Description = p.Description,
    Status = p.Status,
    TaskCount = p.Tasks.Count(t => !t.IsDeleted),
    CompletedTaskCount = p.Tasks.Count(t => t.Status == "Completed" && !t.IsDeleted),
    OverdueTaskCount = p.Tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != "Completed" && !t.IsDeleted)
}).ToListAsync();
```

---

## 🏢 CompanyService Database Operations

### 1. **CREATE Company** (`CreateCompanyAsync`) - SuperAdmin Only
```csharp
var company = new Company
{
    Name = request.Name,
    Domain = request.Domain,
    ContactEmail = request.ContactEmail,
    ContactPhone = request.ContactPhone,
    Address = request.Address,
    SubscriptionType = request.SubscriptionType ?? "Free",
    SubscriptionExpiryDate = request.SubscriptionExpiryDate,
    MaxUsers = request.MaxUsers ?? 10,
    IsActive = true,
    CreatedById = currentUserId
}

await _unitOfWork.Companies.AddAsync(company);
await _unitOfWork.SaveChangesAsync();
```

### 2. **Company Statistics Query**
```csharp
// User statistics by role
var userStats = await _unitOfWork.Users.Query()
    .Where(u => u.CompanyId == companyId)
    .GroupBy(u => u.Role)
    .Select(g => new { Role = g.Key, Count = g.Count() })
    .ToListAsync();

// Project statistics by status
var projectStats = await _unitOfWork.Projects.Query()
    .Where(p => p.CompanyId == companyId)
    .GroupBy(p => p.Status)
    .Select(g => new { Status = g.Key ?? "Unknown", Count = g.Count() })
    .ToListAsync();

// Task statistics
var taskStats = await _unitOfWork.Tasks.Query()
    .Where(t => t.CompanyId == companyId)
    .GroupBy(t => new { t.Status, t.Priority })
    .Select(g => new 
    { 
        Status = g.Key.Status ?? "Unknown",
        Priority = g.Key.Priority ?? "None",
        Count = g.Count() 
    })
    .ToListAsync();
```

---

## 📊 DashboardService Complex Queries

### 1. **Dashboard Overview Query**
```csharp
// Task statistics with multiple conditions
var tasksQuery = _unitOfWork.Tasks.Query()
    .Where(t => !t.IsDeleted)
    .Where(t => t.CompanyId == companyId);

var tasks = await tasksQuery.ToListAsync();

var overview = new DashboardOverviewDto
{
    PendingTasks = tasks.Count(t => t.Status == "Pending"),
    InProgressTasks = tasks.Count(t => t.Status == "InProgress"),
    CompletedTasks = tasks.Count(t => t.Status == "Completed"),
    OverdueTasks = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != "Completed"),
    DueToday = tasks.Count(t => t.DueDate?.Date == DateTime.UtcNow.Date),
    DueThisWeek = tasks.Count(t => t.DueDate >= DateTime.UtcNow && t.DueDate <= DateTime.UtcNow.AddDays(7))
};
```

### 2. **User Performance Query**
```csharp
var userActivities = await _unitOfWork.Tasks.Query()
    .Where(t => t.CompanyId == companyId && t.AssignedToId != null && !t.IsDeleted)
    .Include(t => t.AssignedTo)
    .GroupBy(t => new { t.AssignedToId, t.AssignedTo!.FirstName, t.AssignedTo.LastName })
    .Select(g => new UserActivityDto
    {
        UserId = g.Key.AssignedToId!.Value,
        UserName = $"{g.Key.FirstName} {g.Key.LastName}",
        TasksAssigned = g.Count(),
        TasksCompleted = g.Count(t => t.Status == "Completed"),
        OnTimeDeliveryRate = g.Any(t => t.Status == "Completed") ? 
            (double)g.Count(t => t.Status == "Completed" && t.CompletedDate <= t.DueDate) / 
            g.Count(t => t.Status == "Completed") * 100 : 0
    })
    .OrderByDescending(u => u.TasksCompleted)
    .Take(5)
    .ToListAsync();
```

---

## 🔐 AuthService Database Operations

### 1. **Login Query**
```csharp
var user = await _unitOfWork.Users.Query()
    .Where(u => u.Email == loginRequest.Email)
    .Include(u => u.Company)
    .FirstOrDefaultAsync();

// Password verification
var isValidPassword = BCrypt.Verify(loginRequest.Password, user.PasswordHash);

// Update login information
user.LastLoginAt = DateTime.UtcNow;
user.RefreshToken = GenerateRefreshToken();
user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

_unitOfWork.Users.Update(user);
await _unitOfWork.SaveChangesAsync();
```

### 2. **Register New User**
```csharp
// Check if email exists
var existingUser = await _unitOfWork.Users.Query()
    .AnyAsync(u => u.Email == request.Email);

// Create new user with company assignment
var user = new User
{
    Email = request.Email,
    PasswordHash = BCrypt.HashPassword(request.Password),
    FirstName = request.FirstName,
    LastName = request.LastName,
    CompanyId = request.CompanyId,
    Role = request.Role ?? "User",
    IsActive = true,
    EmailVerified = false,
    CreatedAt = DateTime.UtcNow
}

// Generate email verification token
user.PasswordResetToken = GenerateVerificationToken();
user.PasswordResetExpiry = DateTime.UtcNow.AddHours(24);

await _unitOfWork.Users.AddAsync(user);
await _unitOfWork.SaveChangesAsync();
```

---

## 📎 Additional Database Operations

### 1. **Task Attachments**
```csharp
var attachment = new TaskAttachment
{
    TaskId = taskId,
    FileName = file.FileName,
    FilePath = savedPath,
    FileSize = file.Length,
    ContentType = file.ContentType,
    UploadedById = currentUserId,
    UploadedAt = DateTime.UtcNow
}

await _unitOfWork.TaskAttachments.AddAsync(attachment);
await _unitOfWork.SaveChangesAsync();
```

### 2. **Task Comments**
```csharp
var comment = new TaskComment
{
    TaskId = taskId,
    Comment = request.Comment,
    UserId = currentUserId,
    CreatedAt = DateTime.UtcNow
}

await _unitOfWork.TaskComments.AddAsync(comment);
await _unitOfWork.SaveChangesAsync();
```

### 3. **Notifications**
```csharp
var notification = new Notification
{
    UserId = userId,
    Title = "Task Assigned",
    Message = $"You have been assigned to task: {task.Title}",
    Type = "TaskAssignment",
    RelatedEntityId = task.Id,
    RelatedEntityType = "Task",
    IsRead = false,
    CreatedAt = DateTime.UtcNow
}

await _unitOfWork.Notifications.AddAsync(notification);
await _unitOfWork.SaveChangesAsync();
```

---

## 🔍 Database Query Patterns

### 1. **Multi-Tenant Filtering**
All queries include company filtering (except SuperAdmin):
```csharp
.Where(entity => entity.CompanyId == currentUser.CompanyId)
```

### 2. **Soft Delete Filtering**
Global query filters automatically exclude deleted records:
```csharp
modelBuilder.Entity<Task>().HasQueryFilter(e => !e.IsDeleted);
```

### 3. **Include Patterns**
Related data loading:
```csharp
.Include(t => t.AssignedTo)
.Include(t => t.Client)
.Include(t => t.Project)
.ThenInclude(p => p.Client)
```

### 4. **Pagination Pattern**
```csharp
var totalCount = await query.CountAsync();
var items = await query
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

return new PagedResult<T>
{
    Items = items,
    TotalCount = totalCount,
    PageNumber = pageNumber,
    PageSize = pageSize
};
```

### 5. **Aggregate Queries**
```csharp
// Count by status
.GroupBy(t => t.Status)
.Select(g => new { Status = g.Key, Count = g.Count() })

// Average completion time
.Where(t => t.Status == "Completed")
.Average(t => (t.CompletedDate!.Value - t.CreatedAt).TotalHours)

// Sum of values
.Sum(p => p.Budget ?? 0)
```

---

## 📊 Summary of Database Parameters

### Common Parameters for All Entities:
- **Id**: Guid (Primary Key)
- **CompanyId**: Guid (Multi-tenant identifier)
- **CreatedAt**: DateTime
- **UpdatedAt**: DateTime
- **CreatedById**: Guid (User reference)
- **UpdatedById**: Guid (User reference)
- **IsDeleted**: bool (Soft delete flag)

### Entity-Specific Parameters:

**Task Parameters:**
- Title, Description, TaskNumber
- Priority (Low, Medium, High, Critical)
- Status (Pending, InProgress, Review, Completed, Cancelled)
- Dates (StartDate, DueDate, CompletedDate)
- References (AssignedToId, AssignedById, ClientId, ProjectId)
- Metrics (EstimatedHours, ActualHours, Progress)
- Recurrence (IsRecurring, RecurrencePattern)

**User Parameters:**
- Email, PasswordHash
- FirstName, LastName
- Role (SuperAdmin, CompanyAdmin, Manager, User, TaskAssigner)
- Authentication (RefreshToken, LastLoginAt, EmailVerified)

**Client Parameters:**
- Name, Email, Phone
- ContactPerson, Address, Website
- IsActive

**Project Parameters:**
- Name, Description
- Status (Planning, Active, On Hold, Completed)
- Budget, StartDate, EndDate
- ClientId

**Company Parameters:**
- Name, Domain
- ContactEmail, ContactPhone
- SubscriptionType (Free, Premium, Enterprise)
- SubscriptionExpiryDate, MaxUsers

This comprehensive analysis shows exactly how your services interact with the database, what parameters are sent, and how the data flows through your application.
