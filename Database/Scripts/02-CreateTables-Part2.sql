-- Continue from Part 1
USE TaskManagementDB;
GO

-- 4. SubTasks Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SubTasks' AND xtype='U')
CREATE TABLE Core.SubTasks
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaskId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    IsCompleted BIT DEFAULT 0,
    CompletedAt DATETIME2,
    CompletedById UNIQUEIDENTIFIER,
    SortOrder INT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER,
    UpdatedById UNIQUEIDENTIFIER,
    FOREIGN KEY (TaskId) REFERENCES Core.Tasks(Id) ON DELETE CASCADE,
    FOREIGN KEY (CompletedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id)
);
GO

-- 5. Clients Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Clients' AND xtype='U')
CREATE TABLE Core.Clients
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    ContactPerson NVARCHAR(100),
    Address NVARCHAR(500),
    Website NVARCHAR(200),
    Industry NVARCHAR(50),
    Notes NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER,
    UpdatedById UNIQUEIDENTIFIER,
    FOREIGN KEY (CompanyId) REFERENCES Core.Companies(Id),
    FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id)
);
GO

-- 6. Projects Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Projects' AND xtype='U')
CREATE TABLE Core.Projects
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    ClientId UNIQUEIDENTIFIER,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    ProjectCode NVARCHAR(20) UNIQUE,
    StartDate DATETIME2,
    EndDate DATETIME2,
    Budget DECIMAL(12,2),
    Status NVARCHAR(20) DEFAULT 'Active' CHECK (Status IN ('Planning', 'Active', 'OnHold', 'Completed', 'Cancelled')),
    ProjectManagerId UNIQUEIDENTIFIER,
    TeamMembers NVARCHAR(MAX), -- JSON array of user IDs
    Progress INT DEFAULT 0 CHECK (Progress >= 0 AND Progress <= 100),
    IsArchived BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER,
    UpdatedById UNIQUEIDENTIFIER,
    FOREIGN KEY (CompanyId) REFERENCES Core.Companies(Id),
    FOREIGN KEY (ClientId) REFERENCES Core.Clients(Id),
    FOREIGN KEY (ProjectManagerId) REFERENCES Security.Users(Id),
    FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id)
);
GO