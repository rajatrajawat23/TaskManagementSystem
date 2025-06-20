# 📊 Service Database Operations Analysis
Generated on: Wed Jun 18 15:38:06 IST 2025

## Overview
This document provides detailed analysis of all service methods including:
- Method signatures and parameters
- Database queries and operations
- Entity relationships
- Return types and DTOs

---

## AuthService

### Method: AuthService
```csharp
public AuthService(
```

**Return Type:** `public AuthService`

**Database Operations:**

---

### Method: LoginAsync
```csharp
public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
```

**Parameters:**
- LoginRequestDto loginRequest

**Return Type:** `Task<AuthResponseDto> LoginAsync`

**Database Operations:**

**LINQ Operations:**
- `var foundUser = user.FirstOrDefault();`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: RegisterAsync
```csharp
public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
```

**Parameters:**
- RegisterRequestDto registerRequest

**Return Type:** `Task<AuthResponseDto> RegisterAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: RefreshTokenAsync
```csharp
public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest)
```

**Parameters:**
- RefreshTokenRequestDto refreshTokenRequest

**Return Type:** `Task<AuthResponseDto> RefreshTokenAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: LogoutAsync
```csharp
public async Task<bool> LogoutAsync(Guid userId)
```

**Parameters:**
- Guid userId

**Return Type:** `Task<bool> LogoutAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: ChangePasswordAsync
```csharp
public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto changePasswordRequest)
```

**Parameters:**
- Guid userId
- ChangePasswordRequestDto changePasswordRequest

**Return Type:** `Task<bool> ChangePasswordAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: ForgotPasswordAsync
```csharp
public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequest)
```

**Parameters:**
- ForgotPasswordRequestDto forgotPasswordRequest

**Return Type:** `Task<bool> ForgotPasswordAsync`

**Database Operations:**

**LINQ Operations:**
- `var user = users.FirstOrDefault();`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: ResetPasswordAsync
```csharp
public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequest)
```

**Parameters:**
- ResetPasswordRequestDto resetPasswordRequest

**Return Type:** `Task<bool> ResetPasswordAsync`

**Database Operations:**

**LINQ Operations:**
- `var user = users.FirstOrDefault();`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: VerifyEmailAsync
```csharp
public async Task<bool> VerifyEmailAsync(string token)
```

**Parameters:**
- string token

**Return Type:** `Task<bool> VerifyEmailAsync`

**Database Operations:**

**LINQ Operations:**
- `var user = users.FirstOrDefault();`

**Persistence:** ✅ Calls SaveChangesAsync

---


## ClientService

### Method: ClientService
```csharp
public ClientService(
```

**Return Type:** `public ClientService`

**Database Operations:**

---

### Method: GetClientsAsync
```csharp
public async Task<PagedResult<ClientResponseDto>> GetClientsAsync(
```

**Database Operations:**

**LINQ Operations:**
- `.Include(c => c.Projects)`
- `.Include(c => c.Tasks);`
- `query = query.Where(c => c.CompanyId == currentCompanyId.Value);`
- `query = query.Where(c => c.Name.Contains(search) ||`
- `query = query.Where(c => c.IsActive);`
- `query = query.Where(c => !c.IsActive);`
- `name => sortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),`
- `email => sortDescending ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),`
- `createdat => sortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),`
- `_ => sortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt)`

---

### Method: GetClientByIdAsync
```csharp
public async Task<ClientResponseDto?> GetClientByIdAsync(Guid clientId, Guid companyId)
```

**Parameters:**
- Guid clientId
- Guid companyId

**Return Type:** `Task<ClientResponseDto?> GetClientByIdAsync`

**Database Operations:**

**LINQ Operations:**
- `.Include(c => c.Projects)`
- `.Include(c => c.Tasks)`
- `.Where(c => c.Id == clientId);`
- `query = query.Where(c => c.CompanyId == companyId);`

---

### Method: CreateClientAsync
```csharp
public async Task<ClientResponseDto> CreateClientAsync(CreateClientDto createClientDto)
```

**Parameters:**
- CreateClientDto createClientDto

**Return Type:** `Task<ClientResponseDto> CreateClientAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: UpdateClientAsync
```csharp
public async Task<ClientResponseDto?> UpdateClientAsync(Guid clientId, UpdateClientDto updateClientDto)
```

**Parameters:**
- Guid clientId
- UpdateClientDto updateClientDto

**Return Type:** `Task<ClientResponseDto?> UpdateClientAsync`

**Database Operations:**

**LINQ Operations:**
- `var query = _unitOfWork.Clients.Query().Where(c => c.Id == clientId);`
- `query = query.Where(c => c.CompanyId == companyId.Value);`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: DeleteClientAsync
```csharp
public async Task<bool> DeleteClientAsync(Guid clientId, Guid companyId)
```

**Parameters:**
- Guid clientId
- Guid companyId

**Return Type:** `Task<bool> DeleteClientAsync`

**Database Operations:**

**LINQ Operations:**
- `var query = _unitOfWork.Clients.Query().Where(c => c.Id == clientId);`
- `query = query.Where(c => c.CompanyId == companyId);`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: GetClientProjectsAsync
```csharp
public async Task<IEnumerable<ProjectResponseDto>> GetClientProjectsAsync(Guid clientId, Guid companyId)
```

**Parameters:**
- Guid clientId
- Guid companyId

**Database Operations:**

**LINQ Operations:**
- `.Include(p => p.ProjectManager)`
- `.Include(p => p.Client)`
- `.Where(p => p.ClientId == clientId);`
- `query = query.Where(p => p.CompanyId == companyId);`

---

### Method: GetClientTasksAsync
```csharp
public async Task<IEnumerable<TaskResponseDto>> GetClientTasksAsync(Guid clientId, Guid companyId)
```

**Parameters:**
- Guid clientId
- Guid companyId

**Database Operations:**

**LINQ Operations:**
- `.Include(t => t.AssignedTo)`
- `.Include(t => t.AssignedBy)`
- `.Include(t => t.Client)`
- `.Include(t => t.Project)`
- `.Where(t => t.ClientId == clientId);`
- `query = query.Where(t => t.CompanyId == companyId);`

---


## CompanyService

### Method: CompanyService
```csharp
public CompanyService(
```

**Return Type:** `public CompanyService`

**Database Operations:**

---

### Method: GetAllCompaniesAsync
```csharp
public async Task<PagedResult<CompanyResponseDto>> GetAllCompaniesAsync(
```

**Database Operations:**

**LINQ Operations:**
- `.Include(c => c.Users)`
- `query = query.Where(c => c.Name.Contains(search) ||`
- `query = query.Where(c => c.SubscriptionType == subscriptionType);`
- `query = query.Where(c => c.IsActive == isActive.Value);`
- `.OrderBy(c => c.Name)`

---

### Method: GetCompanyByIdAsync
```csharp
public async Task<CompanyResponseDto> GetCompanyByIdAsync(Guid companyId)
```

**Parameters:**
- Guid companyId

**Return Type:** `Task<CompanyResponseDto> GetCompanyByIdAsync`

**Database Operations:**

**LINQ Operations:**
- `.Include(c => c.Users)`

---

### Method: CreateCompanyAsync
```csharp
public async Task<CompanyResponseDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto)
```

**Parameters:**
- CreateCompanyDto createCompanyDto

**Return Type:** `Task<CompanyResponseDto> CreateCompanyAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: UpdateCompanyAsync
```csharp
public async Task<CompanyResponseDto> UpdateCompanyAsync(Guid companyId, UpdateCompanyDto updateCompanyDto)
```

**Parameters:**
- Guid companyId
- UpdateCompanyDto updateCompanyDto

**Return Type:** `Task<CompanyResponseDto> UpdateCompanyAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: DeleteCompanyAsync
```csharp
public async Task<bool> DeleteCompanyAsync(Guid companyId)
```

**Parameters:**
- Guid companyId

**Return Type:** `Task<bool> DeleteCompanyAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: ActivateCompanyAsync
```csharp
public async Task<bool> ActivateCompanyAsync(Guid companyId)
```

**Parameters:**
- Guid companyId

**Return Type:** `Task<bool> ActivateCompanyAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: DeactivateCompanyAsync
```csharp
public async Task<bool> DeactivateCompanyAsync(Guid companyId)
```

**Parameters:**
- Guid companyId

**Return Type:** `Task<bool> DeactivateCompanyAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: UpdateSubscriptionAsync
```csharp
public async Task<CompanyResponseDto> UpdateSubscriptionAsync(Guid companyId, UpdateSubscriptionDto updateSubscriptionDto)
```

**Parameters:**
- Guid companyId
- UpdateSubscriptionDto updateSubscriptionDto

**Return Type:** `Task<CompanyResponseDto> UpdateSubscriptionAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: GetCompanyStatisticsAsync
```csharp
public async Task<CompanyStatisticsDto> GetCompanyStatisticsAsync(Guid companyId)
```

**Parameters:**
- Guid companyId

**Return Type:** `Task<CompanyStatisticsDto> GetCompanyStatisticsAsync`

**Database Operations:**

**LINQ Operations:**
- `.Include(c => c.Users)`
- `.Where(u => u.CompanyId == companyId)`
- `.Select(g => new { Role = g.Key, Count = g.Count() })`
- `.Where(p => p.CompanyId == companyId)`
- `.Select(g => new { Status = g.Key ?? Unknown, Count = g.Count() })`
- `.Where(t => t.CompanyId == companyId)`
- `.Select(g => new { Status = g.Key ?? Unknown, Count = g.Count() })`
- `.Where(t => t.CompanyId == companyId)`
- `.Select(g => new { Priority = g.Key ?? None, Count = g.Count() })`
- `.Where(t => t.CompanyId == companyId)`
- `.Where(t => t.CompanyId == companyId && t.Status == Completed && t.CompletedDate != null)`
- `.Select(t => new { t.CreatedAt, t.CompletedDate })`
- `if (completedTasksWithTime.Any())`

---

### Method: UpdateSubscriptionAsync
```csharp
public async Task<CompanyResponseDto?> UpdateSubscriptionAsync(UpdateSubscriptionDto dto)
```

**Parameters:**
- UpdateSubscriptionDto dto

**Return Type:** `Task<CompanyResponseDto?> UpdateSubscriptionAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: CanAddMoreUsersAsync
```csharp
public async Task<bool> CanAddMoreUsersAsync(Guid companyId)
```

**Parameters:**
- Guid companyId

**Return Type:** `Task<bool> CanAddMoreUsersAsync`

**Database Operations:**

---

### Method: GetExpiringSubscriptionsAsync
```csharp
public async Task<IEnumerable<CompanyResponseDto>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry)
```

**Parameters:**
- int daysBeforeExpiry

**Database Operations:**

**LINQ Operations:**
- `.Where(c => c.SubscriptionExpiryDate.HasValue &&`
- `.OrderBy(c => c.SubscriptionExpiryDate)`

---


## CurrentUserService

### Method: CurrentUserService
```csharp
public CurrentUserService(IHttpContextAccessor httpContextAccessor)
```

**Parameters:**
- IHttpContextAccessor httpContextAccessor

**Return Type:** `public CurrentUserService`

**Database Operations:**

---


## DashboardService

### Method: DashboardService
```csharp
public DashboardService(IUnitOfWork unitOfWork, ILogger<DashboardService> logger)
```

**Parameters:**
- IUnitOfWork unitOfWork
- ILogger<DashboardService> logger

**Return Type:** `public DashboardService`

**Database Operations:**

---

### Method: GetDashboardAsync
```csharp
public async Task<DashboardDto> GetDashboardAsync(Guid? companyId, Guid? userId, string? userRole, 
```

**Return Type:** `Task<DashboardDto> GetDashboardAsync`

**Database Operations:**

**LINQ Operations:**
- `var tasksQuery = _unitOfWork.Tasks.Query().Where(t => !t.IsDeleted);`
- `tasksQuery = tasksQuery.Where(t => t.CompanyId == companyId.Value);`
- `tasksQuery = tasksQuery.Where(t => t.AssignedToId == userId.Value);`
- `tasksQuery = tasksQuery.Where(t => t.CreatedAt >= startDate.Value);`
- `tasksQuery = tasksQuery.Where(t => t.CreatedAt <= endDate.Value);`
- `PendingTasks = tasks.Count(t => t.Status == Pending),`
- `InProgressTasks = tasks.Count(t => t.Status == InProgress),`
- `CompletedTasks = tasks.Count(t => t.Status == Completed),`
- `OverdueTasks = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != Completed),`
- `DueToday = tasks.Count(t => t.DueDate?.Date == DateTime.UtcNow.Date),`
- `DueThisWeek = tasks.Count(t => t.DueDate >= DateTime.UtcNow && t.DueDate <= DateTime.UtcNow.AddDays(7))`
- `var projectsQuery = _unitOfWork.Projects.Query().Where(p => !p.IsDeleted);`
- `projectsQuery = projectsQuery.Where(p => p.CompanyId == companyId.Value);`
- `ActiveProjects = projects.Count(p => p.Status == Active),`
- `CompletedProjects = projects.Count(p => p.Status == Completed),`
- `OnHoldProjects = projects.Count(p => p.Status == On Hold)`
- `.Where(t => t.DueDate != null && t.DueDate >= DateTime.UtcNow && t.Status != Completed)`
- `.OrderBy(t => t.DueDate)`
- `.Select(t => new UpcomingTaskDto`
- `.Where(t => !string.IsNullOrEmpty(t.Category))`
- `.ToDictionary(g => g.Key, g => g.Count());`
- `var completedTasks = tasks.Where(t => t.Status == Completed).ToList();`
- `TaskCompletionRate = tasks.Any() ? (double)completedTasks.Count / tasks.Count * 100 : 0,`
- `TasksCompletedThisWeek = completedTasks.Count(t => t.CompletedDate >= DateTime.UtcNow.AddDays(-7)),`
- `TasksCompletedThisMonth = completedTasks.Count(t => t.CompletedDate >= DateTime.UtcNow.AddDays(-30)),`
- `OnTimeDeliveryRate = completedTasks.Any() ?`
- `(double)completedTasks.Count(t => t.CompletedDate <= t.DueDate) / completedTasks.Count * 100 : 0,`
- `AverageTaskCompletionTime = completedTasks.Any() ?`

---

### Method: GetCompanyOverviewAsync
```csharp
public async Task<CompanyOverviewDto> GetCompanyOverviewAsync(Guid companyId)
```

**Parameters:**
- Guid companyId

**Return Type:** `Task<CompanyOverviewDto> GetCompanyOverviewAsync`

**Database Operations:**

**LINQ Operations:**
- `.Where(u => u.CompanyId == companyId && !u.IsDeleted)`
- `statistics.ActiveUsers = users.Count(u => u.IsActive);`
- `statistics.UsersByRole = users.GroupBy(u => u.Role).ToDictionary(g => g.Key, g => g.Count());`
- `.Where(p => p.CompanyId == companyId && !p.IsDeleted)`
- `statistics.ActiveProjects = projects.Count(p => p.Status == Active);`
- `.ToDictionary(g => g.Key, g => g.Count());`
- `.Where(c => c.CompanyId == companyId && !c.IsDeleted)`
- `.Where(t => t.CompanyId == companyId && !t.IsDeleted)`
- `statistics.CompletedTasks = tasks.Count(t => t.Status == Completed);`
- `statistics.OverdueTasks = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != Completed);`
- `statistics.TaskCompletionRate = tasks.Any() ? (double)statistics.CompletedTasks / statistics.TotalTasks * 100 : 0;`
- `.ToDictionary(g => g.Key, g => g.Count());`
- `.Where(t => t.CompanyId == companyId && t.AssignedToId != null && !t.IsDeleted)`
- `.Include(t => t.AssignedTo)`
- `.Select(g => new UserActivityDto`
- `TasksCompleted = g.Count(t => t.Status == Completed),`
- `OnTimeDeliveryRate = g.Any(t => t.Status == Completed) ?`
- `(double)g.Count(t => t.Status == Completed && t.CompletedDate <= t.DueDate) /`
- `g.Count(t => t.Status == Completed) * 100 : 0`
- `.Where(p => p.CompanyId == companyId && p.Status == Active && !p.IsDeleted)`
- `.Include(p => p.Tasks)`
- `.Select(p => new ProjectSummaryItemDto`
- `TaskCount = p.Tasks.Count(t => !t.IsDeleted),`
- `CompletedTaskCount = p.Tasks.Count(t => t.Status == Completed && !t.IsDeleted),`
- `CompletionPercentage = p.Tasks.Any(t => !t.IsDeleted) ?`
- `(double)p.Tasks.Count(t => t.Status == Completed && !t.IsDeleted) /`
- `p.Tasks.Count(t => !t.IsDeleted) * 100 : 0`
- `.OrderBy(p => p.DueDate)`

---

### Method: GetSystemOverviewAsync
```csharp
public async Task<SystemOverviewDto> GetSystemOverviewAsync()
```

**Return Type:** `Task<SystemOverviewDto> GetSystemOverviewAsync`

**Database Operations:**

**LINQ Operations:**
- `.Where(c => !c.IsDeleted)`
- `overview.ActiveCompanies = companies.Count(c => c.IsActive);`
- `.ToDictionary(g => g.Key, g => g.Count());`
- `.Where(u => !u.IsDeleted)`
- `overview.ActiveUsers = users.Count(u => u.IsActive);`
- `.Where(t => !t.IsDeleted)`
- `overview.TasksCreatedToday = tasks.Count(t => t.CreatedAt.Date == DateTime.UtcNow.Date);`
- `.Where(c => !c.IsDeleted && c.IsActive)`
- `.Include(c => c.Users)`
- `.Include(c => c.Tasks)`
- `.Include(c => c.Projects)`
- `.Select(c => new CompanyActivityDto`
- `UserCount = c.Users.Count(u => !u.IsDeleted),`
- `TaskCount = c.Tasks.Count(t => !t.IsDeleted),`
- `ProjectCount = c.Projects.Count(p => !p.IsDeleted),`

---

### Method: GetUserPerformanceAsync
```csharp
public async Task<UserPerformanceDto> GetUserPerformanceAsync(Guid userId, Guid companyId, 
```

**Return Type:** `Task<UserPerformanceDto> GetUserPerformanceAsync`

**Database Operations:**

**LINQ Operations:**
- `.Where(t => t.AssignedToId == userId && !t.IsDeleted);`
- `tasksQuery = tasksQuery.Where(t => t.CreatedAt >= startDate.Value);`
- `tasksQuery = tasksQuery.Where(t => t.CreatedAt <= endDate.Value);`
- `performance.TasksCompleted = tasks.Count(t => t.Status == Completed);`
- `performance.TasksInProgress = tasks.Count(t => t.Status == InProgress);`
- `performance.TasksOverdue = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != Completed);`
- `performance.CompletionRate = tasks.Any() ? (double)performance.TasksCompleted / tasks.Count * 100 : 0;`
- `var completedTasks = tasks.Where(t => t.Status == Completed).ToList();`
- `performance.OnTimeDeliveryRate = completedTasks.Any() ?`
- `(double)completedTasks.Count(t => t.CompletedDate <= t.DueDate) / completedTasks.Count * 100 : 0;`
- `performance.AverageCompletionTime = completedTasks.Any() ?`
- `.ToDictionary(g => g.Key, g => g.Count());`
- `.Where(t => !string.IsNullOrEmpty(t.Category))`
- `.ToDictionary(g => g.Key, g => g.Count());`

---

### Method: GetRecentActivitiesAsync
```csharp
public async Task<IEnumerable<ActivityDto>> GetRecentActivitiesAsync(Guid? companyId, Guid? userId, 
```

**Database Operations:**

**LINQ Operations:**
- `.Where(t => !t.IsDeleted)`
- `.Include(t => t.AssignedTo)`
- `.Include(t => t.AssignedBy);`
- `tasksQuery = tasksQuery.Where(t => t.CompanyId == companyId.Value);`
- `tasksQuery = tasksQuery.Where(t => t.AssignedToId == userId.Value || t.AssignedById == userId.Value);`

---


## EmailService

### Method: EmailService
```csharp
public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
```

**Parameters:**
- IConfiguration configuration
- ILogger<EmailService> logger

**Return Type:** `public EmailService`

**Database Operations:**

---

### Method: SendEmailAsync
```csharp
public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
```

**Parameters:**
- string to
- string subject
- string body
- bool isHtml = true

**Return Type:** `Task SendEmailAsync`

**Database Operations:**

---

### Method: SendEmailVerificationAsync
```csharp
public async Task SendEmailVerificationAsync(string email, string verificationToken)
```

**Parameters:**
- string email
- string verificationToken

**Return Type:** `Task SendEmailVerificationAsync`

**Database Operations:**

---

### Method: SendPasswordResetEmailAsync
```csharp
public async Task SendPasswordResetEmailAsync(string email, string resetToken)
```

**Parameters:**
- string email
- string resetToken

**Return Type:** `Task SendPasswordResetEmailAsync`

**Database Operations:**

---

### Method: SendTaskAssignmentEmailAsync
```csharp
public async Task SendTaskAssignmentEmailAsync(string email, string assigneeName, string taskTitle, string taskDescription)
```

**Parameters:**
- string email
- string assigneeName
- string taskTitle
- string taskDescription

**Return Type:** `Task SendTaskAssignmentEmailAsync`

**Database Operations:**

---

### Method: SendTaskStatusUpdateEmailAsync
```csharp
public async Task SendTaskStatusUpdateEmailAsync(string email, string taskTitle, string oldStatus, string newStatus)
```

**Parameters:**
- string email
- string taskTitle
- string oldStatus
- string newStatus

**Return Type:** `Task SendTaskStatusUpdateEmailAsync`

**Database Operations:**

---

### Method: SendTaskReminderEmailAsync
```csharp
public async Task SendTaskReminderEmailAsync(string email, string taskTitle, DateTime dueDate)
```

**Parameters:**
- string email
- string taskTitle
- DateTime dueDate

**Return Type:** `Task SendTaskReminderEmailAsync`

**Database Operations:**

---

### Method: SendWelcomeEmailAsync
```csharp
public async Task SendWelcomeEmailAsync(string email, string userName, string companyName)
```

**Parameters:**
- string email
- string userName
- string companyName

**Return Type:** `Task SendWelcomeEmailAsync`

**Database Operations:**

---


## FileService

### Method: FileService
```csharp
public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
```

**Parameters:**
- IWebHostEnvironment environment
- ILogger<FileService> logger

**Return Type:** `public FileService`

**Database Operations:**

---

### Method: UploadFileAsync
```csharp
public async Task<string> UploadFileAsync(IFormFile file, string folderPath)
```

**Parameters:**
- IFormFile file
- string folderPath

**Return Type:** `Task<string> UploadFileAsync`

**Database Operations:**

---

### Method: DeleteFileAsync
```csharp
public async Task<bool> DeleteFileAsync(string filePath)
```

**Parameters:**
- string filePath

**Return Type:** `Task<bool> DeleteFileAsync`

**Database Operations:**

---

### Method: GetFileAsync
```csharp
public async Task<byte[]> GetFileAsync(string filePath)
```

**Parameters:**
- string filePath

**Return Type:** `Task<byte[]> GetFileAsync`

**Database Operations:**

---

### Method: GetContentType
```csharp
public string GetContentType(string fileName)
```

**Parameters:**
- string fileName

**Return Type:** `string GetContentType`

**Database Operations:**

---

### Method: IsValidFileExtension
```csharp
public bool IsValidFileExtension(string fileName)
```

**Parameters:**
- string fileName

**Return Type:** `bool IsValidFileExtension`

**Database Operations:**

---

### Method: IsValidFileSize
```csharp
public bool IsValidFileSize(long fileSize)
```

**Parameters:**
- long fileSize

**Return Type:** `bool IsValidFileSize`

**Database Operations:**

---


## JwtService

### Method: JwtService
```csharp
public JwtService(IConfiguration configuration)
```

**Parameters:**
- IConfiguration configuration

**Return Type:** `public JwtService`

**Database Operations:**

---

### Method: GenerateAccessToken
```csharp
public string GenerateAccessToken(User user)
```

**Parameters:**
- User user

**Return Type:** `string GenerateAccessToken`

**Database Operations:**

---

### Method: GenerateRefreshToken
```csharp
public string GenerateRefreshToken()
```

**Return Type:** `string GenerateRefreshToken`

**Database Operations:**

---

### Method: GetPrincipalFromExpiredToken
```csharp
public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
```

**Parameters:**
- string token

**Return Type:** `ClaimsPrincipal GetPrincipalFromExpiredToken`

**Database Operations:**

---

### Method: ValidateToken
```csharp
public bool ValidateToken(string token)
```

**Parameters:**
- string token

**Return Type:** `bool ValidateToken`

**Database Operations:**

---


## NotificationService


## ProjectService

### Method: ProjectService
```csharp
public ProjectService(
```

**Return Type:** `public ProjectService`

**Database Operations:**

---

### Method: GetProjectsAsync
```csharp
public async Task<PagedResult<ProjectResponseDto>> GetProjectsAsync(
```

**Database Operations:**

**LINQ Operations:**
- `.Include(p => p.ProjectManager)`
- `.Include(p => p.Client)`
- `.Include(p => p.Tasks);`
- `query = query.Where(p => p.CompanyId == companyId);`
- `query = query.Where(p => p.Name.Contains(search) ||`
- `query = query.Where(p => p.Status == status);`
- `query = query.Where(p => p.ClientId == clientId.Value);`
- `query = query.Where(p => p.ProjectManagerId == projectManagerId.Value);`
- `name => sortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),`
- `projectcode => sortDescending ? query.OrderByDescending(p => p.ProjectCode) : query.OrderBy(p => p.ProjectCode),`
- `status => sortDescending ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status),`
- `startdate => sortDescending ? query.OrderByDescending(p => p.StartDate) : query.OrderBy(p => p.StartDate),`
- `createdat => sortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),`
- `_ => sortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)`

---

### Method: GetProjectByIdAsync
```csharp
public async Task<ProjectResponseDto?> GetProjectByIdAsync(Guid projectId, Guid companyId)
```

**Parameters:**
- Guid projectId
- Guid companyId

**Return Type:** `Task<ProjectResponseDto?> GetProjectByIdAsync`

**Database Operations:**

**LINQ Operations:**
- `.Include(p => p.ProjectManager)`
- `.Include(p => p.Client)`
- `.Include(p => p.Tasks)`
- `.Where(p => p.Id == projectId);`
- `query = query.Where(p => p.CompanyId == companyId);`

---

### Method: CreateProjectAsync
```csharp
public async Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto)
```

**Parameters:**
- CreateProjectDto createProjectDto

**Return Type:** `Task<ProjectResponseDto> CreateProjectAsync`

**Database Operations:**

**LINQ Operations:**
- `if (createProjectDto.TeamMemberIds != null && createProjectDto.TeamMemberIds.Any())`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: UpdateProjectAsync
```csharp
public async Task<ProjectResponseDto?> UpdateProjectAsync(Guid projectId, UpdateProjectDto updateProjectDto)
```

**Parameters:**
- Guid projectId
- UpdateProjectDto updateProjectDto

**Return Type:** `Task<ProjectResponseDto?> UpdateProjectAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: DeleteProjectAsync
```csharp
public async Task<bool> DeleteProjectAsync(Guid projectId, Guid companyId)
```

**Parameters:**
- Guid projectId
- Guid companyId

**Return Type:** `Task<bool> DeleteProjectAsync`

**Database Operations:**

**LINQ Operations:**
- `var query = _unitOfWork.Projects.Query().Where(p => p.Id == projectId);`
- `query = query.Where(p => p.CompanyId == companyId);`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: GetProjectsByClientAsync
```csharp
public async Task<IEnumerable<ProjectResponseDto>> GetProjectsByClientAsync(Guid clientId)
```

**Parameters:**
- Guid clientId

**Database Operations:**

---

### Method: GetProjectsByManagerAsync
```csharp
public async Task<IEnumerable<ProjectResponseDto>> GetProjectsByManagerAsync(Guid managerId)
```

**Parameters:**
- Guid managerId

**Database Operations:**

---

### Method: GetProjectStatisticsAsync
```csharp
public async Task<Dictionary<string, int>> GetProjectStatisticsAsync(Guid companyId)
```

**Parameters:**
- Guid companyId

**Database Operations:**

---

### Method: AddTeamMemberAsync
```csharp
public async Task<ProjectResponseDto?> AddTeamMemberAsync(AddTeamMemberDto dto)
```

**Parameters:**
- AddTeamMemberDto dto

**Return Type:** `Task<ProjectResponseDto?> AddTeamMemberAsync`

**Database Operations:**

**LINQ Operations:**
- `var query = _unitOfWork.Projects.Query().Where(p => p.Id == dto.ProjectId);`
- `query = query.Where(p => p.CompanyId == dto.CompanyId);`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: RemoveTeamMemberAsync
```csharp
public async Task<ProjectResponseDto?> RemoveTeamMemberAsync(Guid projectId, Guid userId, Guid companyId, Guid updatedById)
```

**Parameters:**
- Guid projectId
- Guid userId
- Guid companyId
- Guid updatedById

**Return Type:** `Task<ProjectResponseDto?> RemoveTeamMemberAsync`

**Database Operations:**

**LINQ Operations:**
- `var query = _unitOfWork.Projects.Query().Where(p => p.Id == projectId);`
- `query = query.Where(p => p.CompanyId == companyId);`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: UpdateProjectStatusAsync
```csharp
public async Task<ProjectResponseDto> UpdateProjectStatusAsync(Guid projectId, string status)
```

**Parameters:**
- Guid projectId
- string status

**Return Type:** `Task<ProjectResponseDto> UpdateProjectStatusAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: GetProjectTasksAsync
```csharp
public async Task<IEnumerable<TaskResponseDto>> GetProjectTasksAsync(Guid projectId, Guid companyId)
```

**Parameters:**
- Guid projectId
- Guid companyId

**Database Operations:**

**LINQ Operations:**
- `.Include(t => t.AssignedTo)`
- `.Include(t => t.AssignedBy)`
- `.Where(t => t.ProjectId == projectId);`
- `query = query.Where(t => t.CompanyId == companyId);`

---

### Method: GetProjectStatisticsAsync
```csharp
public async Task<ProjectStatisticsDto> GetProjectStatisticsAsync(Guid projectId, Guid companyId)
```

**Parameters:**
- Guid projectId
- Guid companyId

**Return Type:** `Task<ProjectStatisticsDto> GetProjectStatisticsAsync`

**Database Operations:**

**LINQ Operations:**
- `.Include(p => p.Tasks)`
- `.Where(p => p.Id == projectId);`
- `query = query.Where(p => p.CompanyId == companyId);`
- `var tasks = project.Tasks.Where(t => !t.IsDeleted).ToList();`
- `CompletedTasks = tasks.Count(t => t.Status == Completed),`
- `InProgressTasks = tasks.Count(t => t.Status == InProgress),`
- `PendingTasks = tasks.Count(t => t.Status == Pending),`
- `OverdueTasks = tasks.Count(t => t.Status != Completed && t.DueDate < DateTime.UtcNow),`
- `.ToDictionary(g => g.Key, g => g.Count()),`

---


## TaskService

### Method: TaskService
```csharp
public TaskService(
```

**Return Type:** `public TaskService`

**Database Operations:**

---

### Method: GetTasksAsync
```csharp
public async Task<PagedResult<TaskResponseDto>> GetTasksAsync(
```

**Database Operations:**

**LINQ Operations:**
- `.Include(t => t.AssignedTo)`
- `.Include(t => t.AssignedBy)`
- `.Include(t => t.Client)`
- `.Include(t => t.Project);`
- `query = query.Where(t => t.CompanyId == companyId.Value);`
- `query = query.Where(t => t.Status == status);`
- `query = query.Where(t => t.Priority == priority);`
- `query = query.Where(t => t.AssignedToId == parsedAssignedToId);`
- `query = query.Where(t => t.StartDate >= startDate.Value);`
- `query = query.Where(t => t.DueDate <= endDate.Value);`
- `query = query.Where(t => t.Title.Contains(search) ||`
- `title => sortDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),`
- `priority => sortDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),`
- `status => sortDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),`
- `duedate => sortDescending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),`
- `_ => sortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)`
- `.Select(t => new TaskResponseDto`

---

### Method: GetTaskByIdAsync
```csharp
public async Task<TaskResponseDto?> GetTaskByIdAsync(Guid taskId, Guid? companyId)
```

**Parameters:**
- Guid taskId
- Guid? companyId

**Return Type:** `Task<TaskResponseDto?> GetTaskByIdAsync`

**Database Operations:**

**LINQ Operations:**
- `.Include(t => t.AssignedTo)`
- `.Include(t => t.AssignedBy)`
- `.Include(t => t.Client)`
- `.Include(t => t.Project)`
- `.Include(t => t.SubTasks)`
- `.Include(t => t.Comments).ThenInclude(c => c.User)`
- `.Include(t => t.Attachments).ThenInclude(a => a.UploadedBy)`
- `.Where(t => t.Id == taskId);`
- `query = query.Where(t => t.CompanyId == companyId.Value);`

---

### Method: CreateTaskAsync
```csharp
public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto createTaskRequest)
```

**Parameters:**
- CreateTaskDto createTaskRequest

**Return Type:** `Task<TaskResponseDto> CreateTaskAsync`

**Database Operations:**

**LINQ Operations:**
- `.Where(t => t.CompanyId == task.CompanyId)`
- `.Where(t => t.Id == task.Id)`
- `.Select(t => new TaskResponseDto`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: UpdateTaskAsync
```csharp
public async Task<TaskResponseDto?> UpdateTaskAsync(UpdateTaskDto updateTaskRequest)
```

**Parameters:**
- UpdateTaskDto updateTaskRequest

**Return Type:** `Task<TaskResponseDto?> UpdateTaskAsync`

**Database Operations:**

**LINQ Operations:**
- `var query = _unitOfWork.Tasks.Query().Where(t => t.Id == updateTaskRequest.Id);`
- `query = query.Where(t => t.CompanyId == updateTaskRequest.CompanyId);`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: DeleteTaskAsync
```csharp
public async Task<bool> DeleteTaskAsync(Guid taskId, Guid companyId)
```

**Parameters:**
- Guid taskId
- Guid companyId

**Return Type:** `Task<bool> DeleteTaskAsync`

**Database Operations:**

**LINQ Operations:**
- `var query = _unitOfWork.Tasks.Query().Where(t => t.Id == taskId);`
- `query = query.Where(t => t.CompanyId == companyId);`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: AssignTaskAsync
```csharp
public async Task<TaskResponseDto?> AssignTaskAsync(AssignTaskDto assignTaskDto)
```

**Parameters:**
- AssignTaskDto assignTaskDto

**Return Type:** `Task<TaskResponseDto?> AssignTaskAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: UpdateTaskStatusAsync
```csharp
public async Task<TaskResponseDto?> UpdateTaskStatusAsync(UpdateTaskStatusDto updateStatusDto)
```

**Parameters:**
- UpdateTaskStatusDto updateStatusDto

**Return Type:** `Task<TaskResponseDto?> UpdateTaskStatusAsync`

**Database Operations:**

**LINQ Operations:**
- `var query = _unitOfWork.Tasks.Query().Where(t => t.Id == updateStatusDto.TaskId);`
- `query = query.Where(t => t.CompanyId == updateStatusDto.CompanyId);`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: GetUserTasksAsync
```csharp
public async Task<IEnumerable<TaskResponseDto>> GetUserTasksAsync(Guid userId, Guid companyId)
```

**Parameters:**
- Guid userId
- Guid companyId

**Database Operations:**

**LINQ Operations:**
- `.Include(t => t.AssignedTo)`
- `.Include(t => t.AssignedBy)`
- `.Include(t => t.Client)`
- `.Include(t => t.Project)`
- `.Where(t => t.CompanyId == companyId &&`
- `.OrderBy(t => t.DueDate)`

---

### Method: GetOverdueTasksAsync
```csharp
public async Task<IEnumerable<TaskResponseDto>> GetOverdueTasksAsync(Guid companyId)
```

**Parameters:**
- Guid companyId

**Database Operations:**

**LINQ Operations:**
- `.Include(t => t.AssignedTo)`
- `.Include(t => t.AssignedBy)`
- `.Include(t => t.Client)`
- `.Include(t => t.Project)`
- `.Where(t => t.CompanyId == companyId &&`
- `.OrderBy(t => t.DueDate)`

---

### Method: DuplicateTaskAsync
```csharp
public async Task<TaskResponseDto?> DuplicateTaskAsync(Guid taskId, Guid companyId, Guid userId)
```

**Parameters:**
- Guid taskId
- Guid companyId
- Guid userId

**Return Type:** `Task<TaskResponseDto?> DuplicateTaskAsync`

**Database Operations:**

**LINQ Operations:**
- `.Include(t => t.SubTaskItems)`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: GetTaskStatisticsAsync
```csharp
public async Task<TaskStatisticsDto> GetTaskStatisticsAsync(Guid? companyId)
```

**Parameters:**
- Guid? companyId

**Return Type:** `Task<TaskStatisticsDto> GetTaskStatisticsAsync`

**Database Operations:**

**LINQ Operations:**
- `.Where(t => !t.IsArchived)`
- `.Select(t => new`
- `query = query.Where(t => t.CompanyId == companyId.Value);`
- `PendingTasks = taskData.Count(t => t.Status == Pending),`
- `InProgressTasks = taskData.Count(t => t.Status == InProgress),`
- `CompletedTasks = taskData.Count(t => t.Status == Completed),`
- `OverdueTasks = taskData.Count(t => t.DueDate != null && t.DueDate < DateTime.UtcNow &&`
- `.ToDictionary(g => g.Key, g => g.Count()),`
- `TasksByCategory = taskData.Where(t => !string.IsNullOrEmpty(t.Category))`
- `.ToDictionary(g => g.Key, g => g.Count()),`
- `.ToDictionary(g => g.Key, g => g.Count())`
- `.Where(t => !t.IsArchived && t.AssignedToId != null);`
- `assignmentQuery = assignmentQuery.Where(t => t.CompanyId == companyId.Value);`
- `.Include(t => t.AssignedTo)`
- `.Select(t => new {`
- `.ToDictionary(g => g.Key.Name, g => g.Count());`
- `var completedTasks = taskData.Where(t => t.Status == Completed &&`
- `t.CompletedDate != null).ToList();`
- `if (completedTasks.Any())`
- `if (taskData.Any())`

---

### Method: GetTasksForCalendarAsync
```csharp
public async Task<IEnumerable<TaskCalendarDto>> GetTasksForCalendarAsync(Guid companyId, int year, int month, Guid userId)
```

**Parameters:**
- Guid companyId
- int year
- int month
- Guid userId

**Database Operations:**

**LINQ Operations:**
- `.Include(t => t.AssignedTo)`
- `.Where(t => t.CompanyId == companyId &&`
- `var calendarTasks = tasks.Select(t => new TaskCalendarDto`

---

### Method: AddCommentAsync
```csharp
public async Task<TaskCommentDto?> AddCommentAsync(CreateTaskCommentDto commentDto)
```

**Parameters:**
- CreateTaskCommentDto commentDto

**Return Type:** `Task<TaskCommentDto?> AddCommentAsync`

**Database Operations:**

**LINQ Operations:**
- `.Include(c => c.User)`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: AddAttachmentAsync
```csharp
public async Task<TaskAttachmentDto?> AddAttachmentAsync(Guid taskId, Guid companyId, Guid userId, IFormFile file)
```

**Parameters:**
- Guid taskId
- Guid companyId
- Guid userId
- IFormFile file

**Return Type:** `Task<TaskAttachmentDto?> AddAttachmentAsync`

**Database Operations:**

**LINQ Operations:**
- `.Include(a => a.UploadedBy)`

**Persistence:** ✅ Calls SaveChangesAsync

---


## UserService

### Method: UserService
```csharp
public UserService(
```

**Return Type:** `public UserService`

**Database Operations:**

---

### Method: GetAllUsersAsync
```csharp
public async Task<PagedResult<UserResponseDto>> GetAllUsersAsync(int pageNumber, int pageSize, string? searchTerm = null)
```

**Parameters:**
- int pageNumber
- int pageSize
- string? searchTerm = null

**Database Operations:**

**LINQ Operations:**
- `.Where(u => u.CompanyId == companyId);`
- `query = query.Where(u => u.FirstName.Contains(searchTerm) ||`
- `.OrderBy(u => u.FirstName)`

---

### Method: GetUserByIdAsync
```csharp
public async Task<UserResponseDto> GetUserByIdAsync(Guid userId)
```

**Parameters:**
- Guid userId

**Return Type:** `Task<UserResponseDto> GetUserByIdAsync`

**Database Operations:**

**LINQ Operations:**
- `.Include(u => u.Company)`

---

### Method: CreateUserAsync
```csharp
public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
```

**Parameters:**
- CreateUserDto createUserDto

**Return Type:** `Task<UserResponseDto> CreateUserAsync`

**Database Operations:**

**LINQ Operations:**
- `.Where(c => c.Id == createUserDto.CompanyId)`
- `.Select(c => c.MaxUsers > c.Users.Count(u => u.IsActive))`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: UpdateUserAsync
```csharp
public async Task<UserResponseDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
```

**Parameters:**
- Guid userId
- UpdateUserDto updateUserDto

**Return Type:** `Task<UserResponseDto> UpdateUserAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: DeleteUserAsync
```csharp
public async Task<bool> DeleteUserAsync(Guid userId)
```

**Parameters:**
- Guid userId

**Return Type:** `Task<bool> DeleteUserAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: ActivateUserAsync
```csharp
public async Task<bool> ActivateUserAsync(Guid userId)
```

**Parameters:**
- Guid userId

**Return Type:** `Task<bool> ActivateUserAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: DeactivateUserAsync
```csharp
public async Task<bool> DeactivateUserAsync(Guid userId)
```

**Parameters:**
- Guid userId

**Return Type:** `Task<bool> DeactivateUserAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: UpdateUserRoleAsync
```csharp
public async Task<UserResponseDto> UpdateUserRoleAsync(Guid userId, string newRole)
```

**Parameters:**
- Guid userId
- string newRole

**Return Type:** `Task<UserResponseDto> UpdateUserRoleAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: GetUserTasksAsync
```csharp
public async Task<IEnumerable<TaskResponseDto>> GetUserTasksAsync(Guid userId)
```

**Parameters:**
- Guid userId

**Database Operations:**

---

### Method: GetUserPermissionsAsync
```csharp
public async Task<IEnumerable<UserPermissionDto>> GetUserPermissionsAsync(Guid userId)
```

**Parameters:**
- Guid userId

**Database Operations:**

**LINQ Operations:**
- `.Include(p => p.GrantedBy)`
- `.Where(p => p.UserId == userId)`
- `return permissions.Select(p => new UserPermissionDto`

---

### Method: UpdateUserPermissionsAsync
```csharp
public async Task<bool> UpdateUserPermissionsAsync(Guid userId, UpdateUserPermissionsDto permissions)
```

**Parameters:**
- Guid userId
- UpdateUserPermissionsDto permissions

**Return Type:** `Task<bool> UpdateUserPermissionsAsync`

**Database Operations:**

**LINQ Operations:**
- `.Where(p => p.UserId == userId)`

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: UpdateAvatarAsync
```csharp
public async Task<string> UpdateAvatarAsync(Guid userId, IFormFile avatar)
```

**Parameters:**
- Guid userId
- IFormFile avatar

**Return Type:** `Task<string> UpdateAvatarAsync`

**Database Operations:**

**Persistence:** ✅ Calls SaveChangesAsync

---

### Method: GetUsersByRoleAsync
```csharp
public async Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(string role)
```

**Parameters:**
- string role

**Database Operations:**

---

### Method: GetUsersByCompanyAsync
```csharp
public async Task<IEnumerable<UserResponseDto>> GetUsersByCompanyAsync(Guid companyId)
```

**Parameters:**
- Guid companyId

**Database Operations:**

---


## DTOs and Models

### Request DTOs
- ChangePasswordRequestDto
- ClientDtos
- CompanyDtos
- CreateTaskRequestDto
- ForgotPasswordRequestDto
- LoginRequestDto
- ProjectDtos
- RefreshTokenRequestDto
- RegisterRequestDto
- ResetPasswordRequestDto
- TaskDtos
- TaskFilterRequestDto
- UpdateTaskRequestDto
- UserDtos

### Response DTOs
- AuthResponseDto
- ClientResponseDtos
- CompanyResponseDtos
- DashboardDtos
- NotificationDto
- PagedResponseDto
- ProjectResponseDtos
- TaskResponseDtos
- UserResponseDtos

## Database Context Configuration

### DbSets (Tables)
- public DbSet<Company> Companies { get; set; }
- public DbSet<User> Users { get; set; }
- public DbSet<Core.Entities.Task> Tasks { get; set; }
- public DbSet<SubTask> SubTasks { get; set; }
- public DbSet<Client> Clients { get; set; }
- public DbSet<Project> Projects { get; set; }
- public DbSet<ChatGroup> ChatGroups { get; set; }
- public DbSet<ChatMessage> ChatMessages { get; set; }
- public DbSet<Notification> Notifications { get; set; }
- public DbSet<TaskAttachment> TaskAttachments { get; set; }
- public DbSet<TaskComment> TaskComments { get; set; }
- public DbSet<UserPermission> UserPermissions { get; set; }

