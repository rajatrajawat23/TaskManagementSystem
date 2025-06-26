# Setup database for TaskManagementDB
Write-Host "Setting up TaskManagementDB database..." -ForegroundColor Cyan

# Connection details
$server = "192.168.1.101,1433"
$user = "sa"
$password = "De`$ktop"  # Note the escaped $ character
$database = "TaskManagementDB"

# Paths to SQL scripts
$migrationScript = "C:\Users\Hp\Desktop\Office\trask-tracker-backend\Database\Scripts\00-CompleteMigration.sql"
$seedDataScript = "C:\Users\Hp\Desktop\Office\trask-tracker-backend\Database\Scripts\01-CompleteSeedData.sql"

# Run migration script
Write-Host "Running migration script..." -ForegroundColor Yellow

try {
    # Note: Using -Q for a direct query that creates the database if it doesn't exist
    $createDbCmd = "sqlcmd -S `"$server`" -U `"$user`" -P `"$password`" -Q `"IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = '$database') CREATE DATABASE [$database]`""
    Write-Host "Executing: $createDbCmd" -ForegroundColor Gray
    Invoke-Expression $createDbCmd
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Database created or already exists!" -ForegroundColor Green
    } else {
        Write-Host "Failed to create database with exit code $LASTEXITCODE" -ForegroundColor Red
        Exit $LASTEXITCODE
    }
    
    # Run the migration script
    $migrationCmd = "sqlcmd -S `"$server`" -U `"$user`" -P `"$password`" -d `"$database`" -i `"$migrationScript`""
    Write-Host "Executing: $migrationCmd" -ForegroundColor Gray
    Invoke-Expression $migrationCmd
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Migration script executed successfully!" -ForegroundColor Green
    } else {
        Write-Host "Migration script execution failed with exit code $LASTEXITCODE" -ForegroundColor Red
        Exit $LASTEXITCODE
    }
    
    # Run seed data script
    $seedCmd = "sqlcmd -S `"$server`" -U `"$user`" -P `"$password`" -d `"$database`" -i `"$seedDataScript`""
    Write-Host "Executing: $seedCmd" -ForegroundColor Gray
    Invoke-Expression $seedCmd
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Seed data script executed successfully!" -ForegroundColor Green
    } else {
        Write-Host "Seed data script execution failed with exit code $LASTEXITCODE" -ForegroundColor Red
        Exit $LASTEXITCODE
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
