# Setup database using the SQL scripts
Write-Host "Setting up TaskManagement database..." -ForegroundColor Cyan

# Connection details from your connection string
$server = "192.168.1.101,1433"
$user = "sa"
$password = "De$ktop"
$database = "taskt_racker"

# Paths to SQL scripts
$migrationScript = "C:\Users\Hp\Desktop\Office\trask-tracker-backend\Database\Scripts\00-CompleteMigration.sql"
$seedDataScript = "C:\Users\Hp\Desktop\Office\trask-tracker-backend\Database\Scripts\01-CompleteSeedData.sql"

# Run migration script
Write-Host "Running migration script..." -ForegroundColor Yellow
Write-Host "sqlcmd -S $server -U $user -P *** -i `"$migrationScript`""

try {
    Invoke-Expression "sqlcmd -S `"$server`" -U `"$user`" -P `"$password`" -i `"$migrationScript`""
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Migration script executed successfully!" -ForegroundColor Green
    } else {
        Write-Host "Migration script execution failed with exit code $LASTEXITCODE" -ForegroundColor Red
        Exit $LASTEXITCODE
    }
} catch {
    Write-Host "Error running migration script: $_" -ForegroundColor Red
    Exit 1
}

# Run seed data script
Write-Host "Running seed data script..." -ForegroundColor Yellow
Write-Host "sqlcmd -S $server -U $user -P *** -i `"$seedDataScript`""

try {
    Invoke-Expression "sqlcmd -S `"$server`" -U `"$user`" -P `"$password`" -i `"$seedDataScript`""
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Seed data script executed successfully!" -ForegroundColor Green
    } else {
        Write-Host "Seed data script execution failed with exit code $LASTEXITCODE" -ForegroundColor Red
        Exit $LASTEXITCODE
    }
} catch {
    Write-Host "Error running seed data script: $_" -ForegroundColor Red
    Exit 1
}

Write-Host "Database setup completed successfully!" -ForegroundColor Green
Write-Host "You can now run the application with the existing connection string." -ForegroundColor Cyan

Write-Host "`nTest User Accounts:" -ForegroundColor Magenta
Write-Host "SuperAdmin: superadmin@taskmanagement.com / Admin@123" -ForegroundColor White
Write-Host "Company Admin: john.admin@techinnovations.com / Admin@123" -ForegroundColor White
Write-Host "Manager: sarah.manager@techinnovations.com / Admin@123" -ForegroundColor White
Write-Host "User: mike.user@techinnovations.com / Admin@123" -ForegroundColor White
