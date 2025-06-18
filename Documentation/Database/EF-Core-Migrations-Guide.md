# Entity Framework Core Migrations Guide

## Overview
This project uses a hybrid approach combining SQL scripts for initial database setup and EF Core migrations for ongoing schema changes.

## Migration History

### 1. InitialCreate (20250616072430)
- **Status**: Applied via SQL scripts
- **Description**: Contains all initial tables created through SQL scripts in `/Database/Scripts/`
- **Note**: Manually marked as applied since database was created using SQL scripts

### 2. AddTaskCommentsAndAttachments (20250617091717)
- **Status**: Applied
- **Description**: Updates nullable fields and adds indexes for TaskComments and TaskAttachments
- **Changes**:
  - Made several fields nullable (Phone, Address, Department, etc.)
  - Added indexes for TaskComments (TaskId, CreatedById)
  - Added foreign key relationship for TaskComments.User

## How to Create New Migrations

1. **Add a new migration**:
   ```bash
   cd Backend/TaskManagement.API
   dotnet ef migrations add MigrationName --project ../TaskManagement.Infrastructure --context TaskManagementDbContext
   ```

2. **Apply the migration**:
   ```bash
   dotnet ef database update --project ../TaskManagement.Infrastructure --context TaskManagementDbContext
   ```

3. **Generate SQL script** (optional):
   ```bash
   dotnet ef migrations script --output migration.sql --project ../TaskManagement.Infrastructure --context TaskManagementDbContext
   ```

## Important Notes

1. **Hybrid Approach**: Since the database was initially created using SQL scripts, the first migration was marked as applied without running it.

2. **Connection String**: Ensure the connection string in `appsettings.json` matches your database configuration.

3. **Migration Table**: EF Core tracks applied migrations in the `__EFMigrationsHistory` table.

4. **Rollback**: To rollback to a previous migration:
   ```bash
   dotnet ef database update PreviousMigrationName --project ../TaskManagement.Infrastructure --context TaskManagementDbContext
   ```

## Troubleshooting

### Error: "There is already an object named 'X' in the database"
This occurs when trying to apply a migration that creates objects already created by SQL scripts. Solution:
1. Mark the migration as applied using SQL:
   ```sql
   INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
   VALUES ('MigrationId', '9.0.0');
   ```

### Error: "Cannot drop the index 'X', because it does not exist"
This happens when migrations expect different database structure. Solution:
1. Review the migration file
2. Either modify the migration or apply changes manually via SQL
3. Mark migration as applied

## Best Practices

1. **Always backup** the database before applying migrations
2. **Test migrations** in development environment first
3. **Review generated SQL** before applying to production
4. **Keep migrations small** and focused on specific changes
5. **Document** any manual SQL changes in migration comments

## Migration Files Location
- **Migration Classes**: `/Backend/TaskManagement.Infrastructure/Migrations/`
- **SQL Scripts**: `/Database/Scripts/`
- **EF Tools**: Installed in both API and Infrastructure projects

## Required Packages
- Microsoft.EntityFrameworkCore.SqlServer (9.0.0)
- Microsoft.EntityFrameworkCore.Tools (9.0.0)
- Microsoft.EntityFrameworkCore.Design (9.0.0)
