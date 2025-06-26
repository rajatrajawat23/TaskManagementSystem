# Setup database for TaskManagementDB on local SQL Server instance with fixed paths
Write-Host "Setting up TaskManagementDB database on local SQL Server..." -ForegroundColor Cyan

# Paths to SQL scripts - fixed path with correct slashes
$migrationScript = "C:\Users\Hp\Desktop\Office\trask-tracker\trask-tracker-backend\Database\Scripts\00-CompleteMigration.sql"
$seedDataScript = "C:\Users\Hp\Desktop\Office\trask-tracker\trask-tracker-backend\Database\Scripts\01-CompleteSeedData.sql"

# Connection strings path
$appSettingsPath = "C:\Users\Hp\Desktop\Office\trask-tracker\trask-tracker-backend\Backend\TaskManagement.API\appsettings.json"
$appSettingsDevPath = "C:\Users\Hp\Desktop\Office\trask-tracker\trask-tracker-backend\Backend\TaskManagement.API\appsettings.Development.json"

# Run migration script
Write-Host "Running migration script..." -ForegroundColor Yellow

try {
    # Create the database if it doesn't exist
    $createDbCmd = "sqlcmd -S localhost -E -Q `"IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = 'TaskManagementDB') CREATE DATABASE [TaskManagementDB]`""
    Write-Host "Executing: $createDbCmd" -ForegroundColor Gray
    Invoke-Expression $createDbCmd
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Database created or already exists!" -ForegroundColor Green
    } else {
        Write-Host "Failed to create database with exit code $LASTEXITCODE" -ForegroundColor Red
        Exit $LASTEXITCODE
    }
    
    # Run the migration script
    $migrationCmd = "sqlcmd -S localhost -E -d TaskManagementDB -i `"$migrationScript`""
    Write-Host "Executing: $migrationCmd" -ForegroundColor Gray
    Invoke-Expression $migrationCmd
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Migration script executed successfully!" -ForegroundColor Green
    } else {
        Write-Host "Migration script execution failed with exit code $LASTEXITCODE" -ForegroundColor Red
        Exit $LASTEXITCODE
    }
    
    # Run seed data script
    $seedCmd = "sqlcmd -S localhost -E -d TaskManagementDB -i `"$seedDataScript`""
    Write-Host "Executing: $seedCmd" -ForegroundColor Gray
    Invoke-Expression $seedCmd
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Seed data script executed successfully!" -ForegroundColor Green
    } else {
        Write-Host "Seed data script execution failed with exit code $LASTEXITCODE" -ForegroundColor Red
        Exit $LASTEXITCODE
    }
    
    # Update connection string in appsettings.json
    Write-Host "`nUpdating connection strings in appsettings files..." -ForegroundColor Yellow
    $connectionString = "Server=localhost;Database=TaskManagementDB;Trusted_Connection=True;TrustServerCertificate=true;"
    
    if (Test-Path $appSettingsPath) {
        $content = Get-Content $appSettingsPath -Raw
        $pattern = '"DefaultConnection"\s*:\s*"[^"]*"'
        $replacement = "`"DefaultConnection`": `"$connectionString`""
        $updatedContent = $content -replace $pattern, $replacement
        $updatedContent | Set-Content $appSettingsPath -Force
        Write-Host "Updated connection string in appsettings.json" -ForegroundColor Green
    } else {
        Write-Host "File not found: $appSettingsPath" -ForegroundColor Red
    }
    
    if (Test-Path $appSettingsDevPath) {
        $content = Get-Content $appSettingsDevPath -Raw
        $pattern = '"DefaultConnection"\s*:\s*"[^"]*"'
        $replacement = "`"DefaultConnection`": `"$connectionString`""
        $updatedContent = $content -replace $pattern, $replacement
        $updatedContent | Set-Content $appSettingsDevPath -Force
        Write-Host "Updated connection string in appsettings.Development.json" -ForegroundColor Green
    }
    
    Write-Host "Database setup completed successfully!" -ForegroundColor Green
    
} catch {
    Write-Host "Error setting up database: $_" -ForegroundColor Red
    Exit 1
}

Write-Host "`nTest User Accounts:" -ForegroundColor Magenta
Write-Host "SuperAdmin: superadmin@taskmanagement.com / Admin@123" -ForegroundColor White
Write-Host "Company Admin: john.admin@techinnovations.com / Admin@123" -ForegroundColor White
Write-Host "Manager: sarah.manager@techinnovations.com / Admin@123" -ForegroundColor White
Write-Host "User: mike.user@techinnovations.com / Admin@123" -ForegroundColor White
