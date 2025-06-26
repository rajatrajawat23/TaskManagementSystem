# Create database setup script with alternative options
Write-Host "SQL Server Connection Setup and Database Creation" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan

# Prompt for server details
$defaultServer = "192.168.1.101,1433"
$serverInput = Read-Host "Enter SQL Server address (default: $defaultServer)"
$server = if ([string]::IsNullOrWhiteSpace($serverInput)) { $defaultServer } else { $serverInput }

# Prompt for authentication method
Write-Host "`nChoose authentication method:"
Write-Host "1. Windows Authentication"
Write-Host "2. SQL Server Authentication"
$authMethod = Read-Host "Enter your choice (1 or 2)"

$connectionParams = ""
$connectionString = ""

if ($authMethod -eq "1") {
    # Windows Authentication
    $connectionParams = "-E"
    $connectionString = "Server=$server;Database=taskt_racker;Trusted_Connection=True;TrustServerCertificate=true;Connection Timeout=30;Command Timeout=30000"
} else {
    # SQL Server Authentication
    $user = Read-Host "Enter SQL Server username (default: sa)"
    if ([string]::IsNullOrWhiteSpace($user)) { $user = "sa" }
    
    $securePassword = Read-Host "Enter SQL Server password" -AsSecureString
    $binaryPassword = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($binaryPassword)
    [System.Runtime.InteropServices.Marshal]::ZeroFreeBSTR($binaryPassword)
    
    $connectionParams = "-U `"$user`" -P `"$password`""
    $connectionString = "Server=$server;Database=taskt_racker;User Id=$user;Password=$password;TrustServerCertificate=true;Connection Timeout=30;Command Timeout=30000"
}

# SQL files
$scriptDir = "C:\Users\Hp\Desktop\Office\trask-tracker-backend\Database\Scripts"
$migrationScript = "$scriptDir\00-CompleteMigration.sql"
$seedDataScript = "$scriptDir\01-CompleteSeedData.sql"

# Test connection
Write-Host "`nTesting connection to SQL Server..." -ForegroundColor Yellow
$testCmd = "sqlcmd $connectionParams -Q `"SELECT @@VERSION`" -l 30"
try {
    Invoke-Expression $testCmd | Out-Null
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Connection successful!" -ForegroundColor Green
        
        # Run database scripts
        Write-Host "`nRunning database scripts..." -ForegroundColor Yellow
        
        # Run migration script
        Write-Host "Running migration script..." -ForegroundColor Yellow
        $migrationCmd = "sqlcmd $connectionParams -i `"$migrationScript`""
        Invoke-Expression $migrationCmd
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Migration script executed successfully!" -ForegroundColor Green
            
            # Run seed data script
            Write-Host "`nRunning seed data script..." -ForegroundColor Yellow
            $seedCmd = "sqlcmd $connectionParams -i `"$seedDataScript`""
            Invoke-Expression $seedCmd
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "Seed data script executed successfully!" -ForegroundColor Green
                
                # Update connection string
                Write-Host "`nDo you want to update the connection string in appsettings.json? (Y/N)" -ForegroundColor Yellow
                $updateConfig = Read-Host
                
                if ($updateConfig -eq "Y" -or $updateConfig -eq "y") {
                    $appSettingsPath = "C:\Users\Hp\Desktop\Office\trask-tracker-backend\Backend\TaskManagement.API\appsettings.json"
                    $devSettingsPath = "C:\Users\Hp\Desktop\Office\trask-tracker-backend\Backend\TaskManagement.API\appsettings.Development.json"
                    $prodSettingsPath = "C:\Users\Hp\Desktop\Office\trask-tracker-backend\Backend\TaskManagement.API\appsettings.Production.json"
                    
                    # Function to update connection string in a file
                    function Update-ConnectionString {
                        param (
                            [string]$filePath,
                            [string]$newConnectionString
                        )
                        
                        if (Test-Path $filePath) {
                            $content = Get-Content $filePath -Raw
                            $pattern = '"DefaultConnection"\s*:\s*"[^"]*"'
                            $replacement = "`"DefaultConnection`": `"$newConnectionString`""
                            $updatedContent = $content -replace $pattern, $replacement
                            $updatedContent | Set-Content $filePath -Force
                            Write-Host "Updated connection string in $filePath" -ForegroundColor Green
                        } else {
                            Write-Host "File not found: $filePath" -ForegroundColor Red
                        }
                    }
                    
                    # Update all config files
                    Update-ConnectionString -filePath $appSettingsPath -newConnectionString $connectionString
                    Update-ConnectionString -filePath $devSettingsPath -newConnectionString $connectionString
                    Update-ConnectionString -filePath $prodSettingsPath -newConnectionString $connectionString
                }
                
                Write-Host "`nDatabase setup completed successfully!" -ForegroundColor Green
                Write-Host "`nTest User Accounts:" -ForegroundColor Magenta
                Write-Host "SuperAdmin: superadmin@taskmanagement.com / Admin@123" -ForegroundColor White
                Write-Host "Company Admin: john.admin@techinnovations.com / Admin@123" -ForegroundColor White
                Write-Host "Manager: sarah.manager@techinnovations.com / Admin@123" -ForegroundColor White
                Write-Host "User: mike.user@techinnovations.com / Admin@123" -ForegroundColor White
            } else {
                Write-Host "Seed data script execution failed with exit code $LASTEXITCODE" -ForegroundColor Red
            }
        } else {
            Write-Host "Migration script execution failed with exit code $LASTEXITCODE" -ForegroundColor Red
        }
    } else {
        Write-Host "Connection failed with exit code $LASTEXITCODE" -ForegroundColor Red
    }
} catch {
    Write-Host "Error connecting to SQL Server: $_" -ForegroundColor Red
    
    Write-Host "`nPlease check the following:" -ForegroundColor Yellow
    Write-Host "1. SQL Server is running and accessible from this machine" -ForegroundColor White
    Write-Host "2. The login credentials are correct" -ForegroundColor White
    Write-Host "3. SQL Server is configured to allow SQL Server authentication (if using SQL auth)" -ForegroundColor White
    Write-Host "4. The SQL Server port is open (usually 1433)" -ForegroundColor White
}
