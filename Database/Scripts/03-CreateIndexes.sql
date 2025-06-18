USE TaskManagementDB;
GO

-- Performance Indexes for Companies
CREATE NONCLUSTERED INDEX IX_Companies_Domain ON Core.Companies(Domain) WHERE IsActive = 1;
CREATE NONCLUSTERED INDEX IX_Companies_SubscriptionType ON Core.Companies(SubscriptionType) WHERE IsActive = 1;

-- Performance Indexes for Users
CREATE NONCLUSTERED INDEX IX_Users_Email ON Security.Users(Email) WHERE IsActive = 1;
CREATE NONCLUSTERED INDEX IX_Users_CompanyId ON Security.Users(CompanyId) WHERE IsActive = 1;
CREATE NONCLUSTERED INDEX IX_Users_Role ON Security.Users(Role) WHERE IsActive = 1;
CREATE NONCLUSTERED INDEX IX_Users_CompanyId_Role ON Security.Users(CompanyId, Role) WHERE IsActive = 1;

-- Performance Indexes for Tasks
CREATE NONCLUSTERED INDEX IX_Tasks_CompanyId ON Core.Tasks(CompanyId) WHERE IsArchived = 0;
CREATE NONCLUSTERED INDEX IX_Tasks_AssignedToId ON Core.Tasks(AssignedToId) WHERE IsArchived = 0;
CREATE NONCLUSTERED INDEX IX_Tasks_Status ON Core.Tasks(Status) WHERE IsArchived = 0;
CREATE NONCLUSTERED INDEX IX_Tasks_DueDate ON Core.Tasks(DueDate) WHERE IsArchived = 0;
CREATE NONCLUSTERED INDEX IX_Tasks_CompanyId_Status ON Core.Tasks(CompanyId, Status) WHERE IsArchived = 0;
CREATE NONCLUSTERED INDEX IX_Tasks_AssignedToId_Status ON Core.Tasks(AssignedToId, Status) WHERE IsArchived = 0;
CREATE NONCLUSTERED INDEX IX_Tasks_ProjectId ON Core.Tasks(ProjectId) WHERE ProjectId IS NOT NULL AND IsArchived = 0;
CREATE NONCLUSTERED INDEX IX_Tasks_ClientId ON Core.Tasks(ClientId) WHERE ClientId IS NOT NULL AND IsArchived = 0;

-- Performance Indexes for SubTasks
CREATE NONCLUSTERED INDEX IX_SubTasks_TaskId ON Core.SubTasks(TaskId);
CREATE NONCLUSTERED INDEX IX_SubTasks_TaskId_SortOrder ON Core.SubTasks(TaskId, SortOrder);

-- Performance Indexes for Clients
CREATE NONCLUSTERED INDEX IX_Clients_CompanyId ON Core.Clients(CompanyId) WHERE IsActive = 1;
CREATE NONCLUSTERED INDEX IX_Clients_Name ON Core.Clients(Name) WHERE IsActive = 1;

-- Performance Indexes for Projects
CREATE NONCLUSTERED INDEX IX_Projects_CompanyId ON Core.Projects(CompanyId) WHERE IsArchived = 0;
CREATE NONCLUSTERED INDEX IX_Projects_ClientId ON Core.Projects(ClientId) WHERE IsArchived = 0;
CREATE NONCLUSTERED INDEX IX_Projects_Status ON Core.Projects(Status) WHERE IsArchived = 0;
CREATE NONCLUSTERED INDEX IX_Projects_ProjectManagerId ON Core.Projects(ProjectManagerId) WHERE IsArchived = 0;

-- Performance Indexes for ChatGroups
CREATE NONCLUSTERED INDEX IX_ChatGroups_CompanyId ON Communication.ChatGroups(CompanyId) WHERE IsActive = 1;
CREATE NONCLUSTERED INDEX IX_ChatGroups_RelatedProjectId ON Communication.ChatGroups(RelatedProjectId) WHERE RelatedProjectId IS NOT NULL;

-- Performance Indexes for ChatMessages
CREATE NONCLUSTERED INDEX IX_ChatMessages_GroupId ON Communication.ChatMessages(GroupId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_ChatMessages_SenderId ON Communication.ChatMessages(SenderId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_ChatMessages_CreatedAt ON Communication.ChatMessages(CreatedAt DESC) WHERE IsDeleted = 0;

-- Performance Indexes for Notifications
CREATE NONCLUSTERED INDEX IX_Notifications_UserId ON Communication.Notifications(UserId) WHERE IsRead = 0;
CREATE NONCLUSTERED INDEX IX_Notifications_UserId_CreatedAt ON Communication.Notifications(UserId, CreatedAt DESC);

-- Performance Indexes for TaskAttachments
CREATE NONCLUSTERED INDEX IX_TaskAttachments_TaskId ON Core.TaskAttachments(TaskId);

-- Performance Indexes for TaskComments
CREATE NONCLUSTERED INDEX IX_TaskComments_TaskId ON Core.TaskComments(TaskId);
CREATE NONCLUSTERED INDEX IX_TaskComments_CreatedById ON Core.TaskComments(CreatedById);

-- Performance Indexes for ActivityLogs
CREATE NONCLUSTERED INDEX IX_ActivityLogs_CompanyId ON Core.ActivityLogs(CompanyId);
CREATE NONCLUSTERED INDEX IX_ActivityLogs_UserId ON Core.ActivityLogs(UserId);
CREATE NONCLUSTERED INDEX IX_ActivityLogs_EntityType_EntityId ON Core.ActivityLogs(EntityType, EntityId);
CREATE NONCLUSTERED INDEX IX_ActivityLogs_CreatedAt ON Core.ActivityLogs(CreatedAt DESC);

-- Full-text indexes for search functionality
CREATE FULLTEXT CATALOG TaskManagementCatalog AS DEFAULT;
GO

CREATE FULLTEXT INDEX ON Core.Tasks(Title, Description) KEY INDEX PK__Tasks__3214EC07AA8D2C05;
CREATE FULLTEXT INDEX ON Core.Clients(Name, Notes) KEY INDEX PK__Clients__3214EC079EB21DC5;
CREATE FULLTEXT INDEX ON Core.Projects(Name, Description) KEY INDEX PK__Projects__3214EC07D0F21F4C;
GO