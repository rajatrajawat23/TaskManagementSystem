# 📊 Detailed Database Operations in Services
Generated on: Wed Jun 18 15:39:42 IST 2025

## Summary
This document shows all database operations performed by each service, including:
- Entity queries and filters
- Include statements (joins)
- Add, Update, Delete operations
- SaveChanges calls

---

## AuthService

### Database Context Operations:

### LINQ Operations:

### Operation Count Summary:
- **CREATE operations:** 1
- **READ operations:** 8
- **UPDATE operations:** 9
- **DELETE operations:** 0
0
- **SAVE operations:** 10

---

## ClientService

### Database Context Operations:

### LINQ Operations:
**Line 48:** `query = query.Where(c => c.CompanyId == currentCompanyId.Value);`
**Line 53:** `query = query.Where(c => c.Name.Contains(search) ||`
**Line 62:** `query = query.Where(c => c.IsActive);`
**Line 64:** `query = query.Where(c => !c.IsActive);`
**Line 70:** `"name" => sortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),`
**Line 71:** `"email" => sortDescending ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),`
**Line 72:** `"createdat" => sortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),`
**Line 73:** `_ => sortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt)`
**Line 79:** `.Skip((pageNumber - 1) * pageSize)`
**Line 80:** `.Take(pageSize)`
**Line 112:** `.Where(c => c.Id == clientId);`
**Line 117:** `query = query.Where(c => c.CompanyId == companyId);`
**Line 159:** `var query = _unitOfWork.Clients.Query().Where(c => c.Id == clientId);`
**Line 164:** `query = query.Where(c => c.CompanyId == companyId.Value);`
**Line 196:** `var query = _unitOfWork.Clients.Query().Where(c => c.Id == clientId);`
**Line 201:** `query = query.Where(c => c.CompanyId == companyId);`
**Line 236:** `.Where(p => p.ClientId == clientId);`
**Line 241:** `query = query.Where(p => p.CompanyId == companyId);`
**Line 244:** `var projects = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();`
**Line 268:** `.Where(t => t.ClientId == clientId);`
**Line 273:** `query = query.Where(t => t.CompanyId == companyId);`
**Line 276:** `var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();`

### Operation Count Summary:
- **CREATE operations:** 1
- **READ operations:** 14
- **UPDATE operations:** 2
- **DELETE operations:** 0
0
- **SAVE operations:** 3

---

## CompanyService

### Database Context Operations:

### LINQ Operations:
**Line 42:** `query = query.Where(c => c.Name.Contains(search) || `
**Line 49:** `query = query.Where(c => c.SubscriptionType == subscriptionType);`
**Line 54:** `query = query.Where(c => c.IsActive == isActive.Value);`
**Line 60:** `.OrderBy(c => c.Name)`
**Line 61:** `.Skip((pageNumber - 1) * pageSize)`
**Line 62:** `.Take(pageSize)`
**Line 289:** `.Where(u => u.CompanyId == companyId)`
**Line 290:** `.GroupBy(u => u.Role)`
**Line 291:** `.Select(g => new { Role = g.Key, Count = g.Count() })`
**Line 297:** `.Where(p => p.CompanyId == companyId)`
**Line 298:** `.GroupBy(p => p.Status)`
**Line 299:** `.Select(g => new { Status = g.Key ?? "Unknown", Count = g.Count() })`
**Line 305:** `.Where(t => t.CompanyId == companyId)`
**Line 306:** `.GroupBy(t => t.Status)`
**Line 307:** `.Select(g => new { Status = g.Key ?? "Unknown", Count = g.Count() })`
**Line 313:** `.Where(t => t.CompanyId == companyId)`
**Line 314:** `.GroupBy(t => t.Priority)`
**Line 315:** `.Select(g => new { Priority = g.Key ?? "None", Count = g.Count() })`
**Line 328:** `.Where(t => t.CompanyId == companyId)`
**Line 329:** `.OrderByDescending(t => t.UpdatedAt)`
**Line 335:** `.Where(t => t.CompanyId == companyId && t.Status == "Completed" && t.CompletedDate != null)`
**Line 336:** `.Select(t => new { t.CreatedAt, t.CompletedDate })`
**Line 339:** `if (completedTasksWithTime.Any())`
**Line 341:** `var totalHours = completedTasksWithTime.Sum(t => (t.CompletedDate!.Value - t.CreatedAt).TotalHours);`
**Line 404:** `.Where(c => c.SubscriptionExpiryDate.HasValue && `
**Line 407:** `.OrderBy(c => c.SubscriptionExpiryDate)`

### Operation Count Summary:
- **CREATE operations:** 1
- **READ operations:** 10
- **UPDATE operations:** 6
- **DELETE operations:** 0
0
- **SAVE operations:** 7

---

## CurrentUserService

### Database Context Operations:

### LINQ Operations:

### Operation Count Summary:
- **CREATE operations:** 0
0
- **READ operations:** 0
0
- **UPDATE operations:** 0
0
- **DELETE operations:** 0
0
- **SAVE operations:** 0
0

---

## DashboardService

### Database Context Operations:

### LINQ Operations:
**Line 27:** `var tasksQuery = _unitOfWork.Tasks.Query().Where(t => !t.IsDeleted);`
**Line 30:** `tasksQuery = tasksQuery.Where(t => t.CompanyId == companyId.Value);`
**Line 33:** `tasksQuery = tasksQuery.Where(t => t.AssignedToId == userId.Value);`
**Line 36:** `tasksQuery = tasksQuery.Where(t => t.CreatedAt >= startDate.Value);`
**Line 39:** `tasksQuery = tasksQuery.Where(t => t.CreatedAt <= endDate.Value);`
**Line 46:** `PendingTasks = tasks.Count(t => t.Status == "Pending"),`
**Line 47:** `InProgressTasks = tasks.Count(t => t.Status == "InProgress"),`
**Line 48:** `CompletedTasks = tasks.Count(t => t.Status == "Completed"),`
**Line 49:** `OverdueTasks = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != "Completed"),`
**Line 50:** `DueToday = tasks.Count(t => t.DueDate?.Date == DateTime.UtcNow.Date),`
**Line 51:** `DueThisWeek = tasks.Count(t => t.DueDate >= DateTime.UtcNow && t.DueDate <= DateTime.UtcNow.AddDays(7))`
**Line 55:** `var projectsQuery = _unitOfWork.Projects.Query().Where(p => !p.IsDeleted);`
**Line 58:** `projectsQuery = projectsQuery.Where(p => p.CompanyId == companyId.Value);`
**Line 65:** `ActiveProjects = projects.Count(p => p.Status == "Active"),`
**Line 66:** `CompletedProjects = projects.Count(p => p.Status == "Completed"),`
**Line 67:** `OnHoldProjects = projects.Count(p => p.Status == "On Hold")`
**Line 72:** `.Where(t => t.DueDate != null && t.DueDate >= DateTime.UtcNow && t.Status != "Completed")`
**Line 73:** `.OrderBy(t => t.DueDate)`
**Line 74:** `.Take(10);`
**Line 77:** `.Select(t => new UpcomingTaskDto`
**Line 90:** `.Where(t => !string.IsNullOrEmpty(t.Category))`
**Line 91:** `.GroupBy(t => t.Category!)`
**Line 92:** `.ToDictionary(g => g.Key, g => g.Count());`
**Line 95:** `var completedTasks = tasks.Where(t => t.Status == "Completed").ToList();`
**Line 99:** `TaskCompletionRate = tasks.Any() ? (double)completedTasks.Count / tasks.Count * 100 : 0,`
**Line 100:** `TasksCompletedThisWeek = completedTasks.Count(t => t.CompletedDate >= DateTime.UtcNow.AddDays(-7)),`
**Line 101:** `TasksCompletedThisMonth = completedTasks.Count(t => t.CompletedDate >= DateTime.UtcNow.AddDays(-30)),`
**Line 102:** `OnTimeDeliveryRate = completedTasks.Any() ? `
**Line 103:** `(double)completedTasks.Count(t => t.CompletedDate <= t.DueDate) / completedTasks.Count * 100 : 0,`
**Line 104:** `AverageTaskCompletionTime = completedTasks.Any() ? `
**Line 105:** `completedTasks.Average(t => (t.CompletedDate!.Value - t.CreatedAt).TotalHours) : 0`
**Line 127:** `.Where(u => u.CompanyId == companyId && !u.IsDeleted)`
**Line 131:** `statistics.ActiveUsers = users.Count(u => u.IsActive);`
**Line 132:** `statistics.UsersByRole = users.GroupBy(u => u.Role).ToDictionary(g => g.Key, g => g.Count());`
**Line 135:** `.Where(p => p.CompanyId == companyId && !p.IsDeleted)`
**Line 139:** `statistics.ActiveProjects = projects.Count(p => p.Status == "Active");`
**Line 140:** `statistics.ProjectsByStatus = projects.GroupBy(p => p.Status ?? "Unknown")`
**Line 141:** `.ToDictionary(g => g.Key, g => g.Count());`
**Line 144:** `.Where(c => c.CompanyId == companyId && !c.IsDeleted)`
**Line 150:** `.Where(t => t.CompanyId == companyId && !t.IsDeleted)`
**Line 154:** `statistics.CompletedTasks = tasks.Count(t => t.Status == "Completed");`
**Line 155:** `statistics.OverdueTasks = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != "Completed");`
**Line 156:** `statistics.TaskCompletionRate = tasks.Any() ? (double)statistics.CompletedTasks / statistics.TotalTasks * 100 : 0;`
**Line 157:** `statistics.TasksByStatus = tasks.GroupBy(t => t.Status ?? "Unknown")`
**Line 158:** `.ToDictionary(g => g.Key, g => g.Count());`
**Line 164:** `.Where(t => t.CompanyId == companyId && t.AssignedToId != null && !t.IsDeleted)`
**Line 166:** `.GroupBy(t => new { t.AssignedToId, t.AssignedTo!.FirstName, t.AssignedTo.LastName })`
**Line 167:** `.Select(g => new UserActivityDto`
**Line 171:** `TasksCompleted = g.Count(t => t.Status == "Completed"),`
**Line 172:** `OnTimeDeliveryRate = g.Any(t => t.Status == "Completed") ? `
**Line 173:** `(double)g.Count(t => t.Status == "Completed" && t.CompletedDate <= t.DueDate) / `
**Line 174:** `g.Count(t => t.Status == "Completed") * 100 : 0`
**Line 176:** `.OrderByDescending(u => u.TasksCompleted)`
**Line 177:** `.Take(5)`
**Line 184:** `.Where(p => p.CompanyId == companyId && p.Status == "Active" && !p.IsDeleted)`
**Line 186:** `.Select(p => new ProjectSummaryItemDto`
**Line 192:** `TaskCount = p.Tasks.Count(t => !t.IsDeleted),`
**Line 193:** `CompletedTaskCount = p.Tasks.Count(t => t.Status == "Completed" && !t.IsDeleted),`
**Line 194:** `CompletionPercentage = p.Tasks.Any(t => !t.IsDeleted) ? `
**Line 195:** `(double)p.Tasks.Count(t => t.Status == "Completed" && !t.IsDeleted) / `
**Line 196:** `p.Tasks.Count(t => !t.IsDeleted) * 100 : 0`
**Line 198:** `.OrderBy(p => p.DueDate)`
**Line 199:** `.Take(10)`
**Line 220:** `.Where(c => !c.IsDeleted)`
**Line 224:** `overview.ActiveCompanies = companies.Count(c => c.IsActive);`
**Line 226:** `.GroupBy(c => c.SubscriptionType ?? "Free")`
**Line 227:** `.ToDictionary(g => g.Key, g => g.Count());`
**Line 230:** `.Where(u => !u.IsDeleted)`
**Line 234:** `overview.ActiveUsers = users.Count(u => u.IsActive);`
**Line 237:** `.Where(t => !t.IsDeleted)`
**Line 241:** `overview.TasksCreatedToday = tasks.Count(t => t.CreatedAt.Date == DateTime.UtcNow.Date);`
**Line 245:** `.Where(c => !c.IsDeleted && c.IsActive)`
**Line 249:** `.Select(c => new CompanyActivityDto`
**Line 253:** `UserCount = c.Users.Count(u => !u.IsDeleted),`
**Line 254:** `TaskCount = c.Tasks.Count(t => !t.IsDeleted),`
**Line 255:** `ProjectCount = c.Projects.Count(p => !p.IsDeleted),`
**Line 258:** `.OrderByDescending(c => c.TaskCount)`
**Line 259:** `.Take(10)`
**Line 291:** `.Where(t => t.AssignedToId == userId && !t.IsDeleted);`
**Line 294:** `tasksQuery = tasksQuery.Where(t => t.CreatedAt >= startDate.Value);`
**Line 297:** `tasksQuery = tasksQuery.Where(t => t.CreatedAt <= endDate.Value);`
**Line 302:** `performance.TasksCompleted = tasks.Count(t => t.Status == "Completed");`
**Line 303:** `performance.TasksInProgress = tasks.Count(t => t.Status == "InProgress");`
**Line 304:** `performance.TasksOverdue = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != "Completed");`
**Line 305:** `performance.CompletionRate = tasks.Any() ? (double)performance.TasksCompleted / tasks.Count * 100 : 0;`
**Line 307:** `var completedTasks = tasks.Where(t => t.Status == "Completed").ToList();`
**Line 308:** `performance.OnTimeDeliveryRate = completedTasks.Any() ? `
**Line 309:** `(double)completedTasks.Count(t => t.CompletedDate <= t.DueDate) / completedTasks.Count * 100 : 0;`
**Line 310:** `performance.AverageCompletionTime = completedTasks.Any() ? `
**Line 311:** `completedTasks.Average(t => (t.CompletedDate!.Value - t.CreatedAt).TotalHours) : 0;`
**Line 314:** `.GroupBy(t => t.Priority ?? "Medium")`
**Line 315:** `.ToDictionary(g => g.Key, g => g.Count());`
**Line 318:** `.Where(t => !string.IsNullOrEmpty(t.Category))`
**Line 319:** `.GroupBy(t => t.Category!)`
**Line 320:** `.ToDictionary(g => g.Key, g => g.Count());`
**Line 340:** `.Where(t => !t.IsDeleted)`
**Line 345:** `tasksQuery = tasksQuery.Where(t => t.CompanyId == companyId.Value);`
**Line 348:** `tasksQuery = tasksQuery.Where(t => t.AssignedToId == userId.Value || t.AssignedById == userId.Value);`
**Line 351:** `.OrderByDescending(t => t.UpdatedAt)`
**Line 352:** `.Take(count)`
**Line 371:** `return activities.OrderByDescending(a => a.Timestamp).Take(count);`

### Operation Count Summary:
- **CREATE operations:** 1
- **READ operations:** 28
- **UPDATE operations:** 0
0
- **DELETE operations:** 0
0
- **SAVE operations:** 0
0

---

## EmailService

### Database Context Operations:

### LINQ Operations:

### Operation Count Summary:
- **CREATE operations:** 1
- **READ operations:** 0
0
- **UPDATE operations:** 0
0
- **DELETE operations:** 0
0
- **SAVE operations:** 0
0

---

## FileService

### Database Context Operations:

### LINQ Operations:

### Operation Count Summary:
- **CREATE operations:** 0
0
- **READ operations:** 0
0
- **UPDATE operations:** 0
0
- **DELETE operations:** 1
- **SAVE operations:** 0
0

---

## JwtService

### Database Context Operations:

### LINQ Operations:

### Operation Count Summary:
- **CREATE operations:** 1
- **READ operations:** 0
0
- **UPDATE operations:** 0
0
- **DELETE operations:** 0
0
- **SAVE operations:** 0
0

---

## NotificationService

### Database Context Operations:

### LINQ Operations:

### Operation Count Summary:
- **CREATE operations:** 0
0
- **READ operations:** 0
0
- **UPDATE operations:** 0
0
- **DELETE operations:** 0
0
- **SAVE operations:** 0
0

---

## ProjectService

### Database Context Operations:

### LINQ Operations:
**Line 50:** `query = query.Where(p => p.CompanyId == companyId);`
**Line 55:** `query = query.Where(p => p.Name.Contains(search) ||`
**Line 62:** `query = query.Where(p => p.Status == status);`
**Line 67:** `query = query.Where(p => p.ClientId == clientId.Value);`
**Line 72:** `query = query.Where(p => p.ProjectManagerId == projectManagerId.Value);`
**Line 78:** `"name" => sortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),`
**Line 79:** `"projectcode" => sortDescending ? query.OrderByDescending(p => p.ProjectCode) : query.OrderBy(p => p.ProjectCode),`
**Line 80:** `"status" => sortDescending ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status),`
**Line 81:** `"startdate" => sortDescending ? query.OrderByDescending(p => p.StartDate) : query.OrderBy(p => p.StartDate),`
**Line 82:** `"createdat" => sortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),`
**Line 83:** `_ => sortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)`
**Line 89:** `.Skip((pageNumber - 1) * pageSize)`
**Line 90:** `.Take(pageSize)`
**Line 123:** `.Where(p => p.Id == projectId);`
**Line 128:** `query = query.Where(p => p.CompanyId == companyId);`
**Line 157:** `if (createProjectDto.TeamMemberIds != null && createProjectDto.TeamMemberIds.Any())`
**Line 212:** `var query = _unitOfWork.Projects.Query().Where(p => p.Id == projectId);`
**Line 217:** `query = query.Where(p => p.CompanyId == companyId);`
**Line 292:** `var query = _unitOfWork.Projects.Query().Where(p => p.Id == dto.ProjectId);`
**Line 297:** `query = query.Where(p => p.CompanyId == dto.CompanyId);`
**Line 338:** `var query = _unitOfWork.Projects.Query().Where(p => p.Id == projectId);`
**Line 343:** `query = query.Where(p => p.CompanyId == companyId);`
**Line 413:** `.Where(t => t.ProjectId == projectId);`
**Line 418:** `query = query.Where(t => t.CompanyId == companyId);`
**Line 421:** `var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();`
**Line 442:** `.Where(p => p.Id == projectId);`
**Line 447:** `query = query.Where(p => p.CompanyId == companyId);`
**Line 455:** `var tasks = project.Tasks.Where(t => !t.IsDeleted).ToList();`
**Line 462:** `CompletedTasks = tasks.Count(t => t.Status == "Completed"),`
**Line 463:** `InProgressTasks = tasks.Count(t => t.Status == "InProgress"),`
**Line 464:** `PendingTasks = tasks.Count(t => t.Status == "Pending"),`
**Line 465:** `OverdueTasks = tasks.Count(t => t.Status != "Completed" && t.DueDate < DateTime.UtcNow),`
**Line 470:** `TasksByPriority = tasks.GroupBy(t => t.Priority ?? "Medium")`
**Line 471:** `.ToDictionary(g => g.Key, g => g.Count()),`
**Line 492:** `var totalEstimatedHours = tasks.Sum(t => t.EstimatedHours ?? 0);`
**Line 493:** `var totalActualHours = tasks.Sum(t => t.ActualHours ?? 0);`
**Line 503:** `var completedTasks = tasks.Where(t => t.Status == "Completed" && t.CompletedDate.HasValue).ToList();`
**Line 504:** `if (!completedTasks.Any())`
**Line 507:** `var totalDays = completedTasks.Sum(t => (t.CompletedDate!.Value - t.CreatedAt).TotalDays);`
**Line 515:** `.Where(t => t.ProjectId == projectId && !t.IsDeleted)`
**Line 518:** `return tasks.Where(t => t.AssignedTo != null)`
**Line 519:** `.GroupBy(t => $"{t.AssignedTo!.FirstName} {t.AssignedTo.LastName}")`
**Line 520:** `.ToDictionary(g => g.Key, g => g.Sum(t => t.ActualHours ?? 0));`
**Line 528:** `.Where(t => t.ProjectId == projectId && !t.IsDeleted)`
**Line 534:** `var taskGroups = tasks.GroupBy(t => t.Category ?? "General").ToList();`
**Line 538:** `var groupTasks = group.ToList();`
**Line 539:** `if (groupTasks.Any())`
**Line 544:** `DueDate = groupTasks.Max(t => t.DueDate ?? DateTime.UtcNow.AddDays(30)),`
**Line 545:** `IsCompleted = groupTasks.All(t => t.Status == "Completed"),`
**Line 546:** `CompletionPercentage = (int)(groupTasks.Count(t => t.Status == "Completed") * 100.0 / groupTasks.Count)`
**Line 551:** `return milestones.OrderBy(m => m.DueDate).ToList();`

### Operation Count Summary:
- **CREATE operations:** 3
- **READ operations:** 25
- **UPDATE operations:** 5
- **DELETE operations:** 1
- **SAVE operations:** 6

---

## TaskService

### Database Context Operations:

### LINQ Operations:
**Line 57:** `query = query.Where(t => t.CompanyId == companyId.Value);`
**Line 62:** `query = query.Where(t => t.Status == status);`
**Line 65:** `query = query.Where(t => t.Priority == priority);`
**Line 68:** `query = query.Where(t => t.AssignedToId == parsedAssignedToId);`
**Line 71:** `query = query.Where(t => t.StartDate >= startDate.Value);`
**Line 74:** `query = query.Where(t => t.DueDate <= endDate.Value);`
**Line 78:** `query = query.Where(t => t.Title.Contains(search) || `
**Line 88:** `"title" => sortDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),`
**Line 89:** `"priority" => sortDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),`
**Line 90:** `"status" => sortDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),`
**Line 91:** `"duedate" => sortDescending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),`
**Line 92:** `_ => sortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)`
**Line 97:** `query = query.OrderByDescending(t => t.CreatedAt);`
**Line 103:** `.Skip((pageNumber - 1) * pageSize)`
**Line 104:** `.Take(pageSize)`
**Line 105:** `.Select(t => new TaskResponseDto`
**Line 164:** `.Where(t => t.Id == taskId);`
**Line 169:** `query = query.Where(t => t.CompanyId == companyId.Value);`
**Line 198:** `.Where(t => t.CompanyId == task.CompanyId)`
**Line 207:** `.Where(t => t.Id == task.Id)`
**Line 208:** `.Select(t => new TaskResponseDto`
**Line 245:** `var query = _unitOfWork.Tasks.Query().Where(t => t.Id == updateTaskRequest.Id);`
**Line 250:** `query = query.Where(t => t.CompanyId == updateTaskRequest.CompanyId);`
**Line 291:** `var query = _unitOfWork.Tasks.Query().Where(t => t.Id == taskId);`
**Line 295:** `query = query.Where(t => t.CompanyId == companyId);`
**Line 412:** `var query = _unitOfWork.Tasks.Query().Where(t => t.Id == updateStatusDto.TaskId);`
**Line 417:** `query = query.Where(t => t.CompanyId == updateStatusDto.CompanyId);`
**Line 519:** `.Where(t => t.CompanyId == companyId && `
**Line 524:** `.OrderBy(t => t.DueDate)`
**Line 548:** `.Where(t => t.CompanyId == companyId && `
**Line 554:** `.OrderBy(t => t.DueDate)`
**Line 642:** `.Where(t => !t.IsArchived)`
**Line 643:** `.Select(t => new `
**Line 658:** `query = query.Where(t => t.CompanyId == companyId.Value);`
**Line 666:** `PendingTasks = taskData.Count(t => t.Status == "Pending"),`
**Line 667:** `InProgressTasks = taskData.Count(t => t.Status == "InProgress"),`
**Line 668:** `CompletedTasks = taskData.Count(t => t.Status == "Completed"),`
**Line 669:** `OverdueTasks = taskData.Count(t => t.DueDate != null && t.DueDate < DateTime.UtcNow && `
**Line 671:** `TasksByPriority = taskData.GroupBy(t => t.Priority)`
**Line 672:** `.ToDictionary(g => g.Key, g => g.Count()),`
**Line 673:** `TasksByCategory = taskData.Where(t => !string.IsNullOrEmpty(t.Category))`
**Line 674:** `.GroupBy(t => t.Category)`
**Line 675:** `.ToDictionary(g => g.Key, g => g.Count()),`
**Line 676:** `TasksByStatus = taskData.GroupBy(t => t.Status)`
**Line 677:** `.ToDictionary(g => g.Key, g => g.Count())`
**Line 682:** `.Where(t => !t.IsArchived && t.AssignedToId != null);`
**Line 686:** `assignmentQuery = assignmentQuery.Where(t => t.CompanyId == companyId.Value);`
**Line 691:** `.Select(t => new { `
**Line 698:** `.GroupBy(t => new { t.AssignedToId, t.Name })`
**Line 699:** `.ToDictionary(g => g.Key.Name, g => g.Count());`
**Line 704:** `var completedTasks = taskData.Where(t => t.Status == "Completed" && `
**Line 705:** `t.CompletedDate != null).ToList();`
**Line 707:** `if (completedTasks.Any())`
**Line 709:** `var totalHours = completedTasks.Sum(t => (t.CompletedDate!.Value - t.CreatedAt).TotalHours);`
**Line 714:** `if (taskData.Any())`
**Line 737:** `.Where(t => t.CompanyId == companyId &&`
**Line 746:** `var calendarTasks = tasks.Select(t => new TaskCalendarDto`

### Operation Count Summary:
- **CREATE operations:** 7
- **READ operations:** 27
- **UPDATE operations:** 4
- **DELETE operations:** 0
0
- **SAVE operations:** 8

---

## UserService

### Database Context Operations:

### LINQ Operations:
**Line 37:** `.Where(u => u.CompanyId == companyId);`
**Line 41:** `query = query.Where(u => u.FirstName.Contains(searchTerm) ||`
**Line 49:** `.OrderBy(u => u.FirstName)`
**Line 51:** `.Skip((pageNumber - 1) * pageSize)`
**Line 52:** `.Take(pageSize)`
**Line 104:** `.Where(c => c.Id == createUserDto.CompanyId)`
**Line 105:** `.Select(c => c.MaxUsers > c.Users.Count(u => u.IsActive))`
**Line 271:** `.Where(p => p.UserId == userId)`
**Line 274:** `return permissions.Select(p => new UserPermissionDto`
**Line 295:** `.Where(p => p.UserId == userId)`

### Operation Count Summary:
- **CREATE operations:** 2
- **READ operations:** 5
- **UPDATE operations:** 6
- **DELETE operations:** 1
- **SAVE operations:** 8

---

## Overall Summary

### Total Database Operations Across All Services:
- **Total CREATE operations:** 1
- **Total READ operations:** 8
- **Total UPDATE operations:** 9
- **Total DELETE operations:** 0
- **Total SAVE operations:** 0

### Services Analyzed:
- AuthService
- ClientService
- CompanyService
- CurrentUserService
- DashboardService
- EmailService
- FileService
- JwtService
- NotificationService
- ProjectService
- TaskService
- UserService
