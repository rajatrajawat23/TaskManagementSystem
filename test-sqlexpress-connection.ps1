# Test connection to SQL Server Express
Write-Host "Testing connection to SQL Server Express..." -ForegroundColor Cyan

# Connection details
$server = "192.168.1.101\SQLEXPRESS"
$user = "sa"
$password = "De$ktop"
$database = "TaskManagementDB"

# Test 1: Basic connection to the server
Write-Host "`nTest 1: Testing basic connection to server..." -ForegroundColor Yellow
$testCmd = "sqlcmd -S `"$server`" -U `"$user`" -P `"$password`" -Q `"SELECT @@SERVERNAME AS 'Server Name', @@VERSION AS 'SQL Server Version'`" -l 30"
Write-Host "Executing: $testCmd" -ForegroundColor Gray

try {
    Invoke-Expression $testCmd
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Basic server connection successful!" -ForegroundColor Green
    } else {
        Write-Host "✗ Basic server connection failed with exit code $LASTEXITCODE" -ForegroundColor Red
        Write-Host "`nPossible causes:" -ForegroundColor Yellow
        Write-Host "1. Server name is incorrect - Check that '$server' is correct" -ForegroundColor White
        Write-Host "2. SQL Authentication is not enabled on the server" -ForegroundColor White
        Write-Host "3. 'sa' account is disabled or password is incorrect" -ForegroundColor White
        Write-Host "4. Firewall is blocking the connection" -ForegroundColor White
    }
} catch {
    Write-Host "✗ Error: $_" -ForegroundColor Red
}

# Test 2: Try Windows Authentication
Write-Host "`nTest 2: Testing Windows Authentication..." -ForegroundColor Yellow
$winAuthCmd = "sqlcmd -S `"$server`" -E -Q `"SELECT @@SERVERNAME AS 'Server Name'`" -l 30"
Write-Host "Executing: $winAuthCmd" -ForegroundColor Gray

try {
    Invoke-Expression $winAuthCmd
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Windows Authentication connection successful!" -ForegroundColor Green
        Write-Host "Consider using Windows Authentication instead of SQL Authentication" -ForegroundColor Yellow
    } else {
        Write-Host "✗ Windows Authentication connection failed with exit code $LASTEXITCODE" -ForegroundColor Red
    }
} catch {
    Write-Host "✗ Error: $_" -ForegroundColor Red
}

# Test 3: Check if server can be pinged
Write-Host "`nTest 3: Testing network connectivity..." -ForegroundColor Yellow
$serverAddress = $server.Split('\')[0]  # Extract server address without instance name
$pingCmd = "ping -n 4 $serverAddress"
Write-Host "Executing: $pingCmd" -ForegroundColor Gray

try {
    Invoke-Expression $pingCmd
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Server can be pinged successfully!" -ForegroundColor Green
    } else {
        Write-Host "✗ Cannot ping server. Network connectivity issue." -ForegroundColor Red
    }
} catch {
    Write-Host "✗ Error: $_" -ForegroundColor Red
}

Write-Host "`nConnection Information:" -ForegroundColor Cyan
Write-Host "Server: $server" -ForegroundColor White
Write-Host "User: $user" -ForegroundColor White
Write-Host "Password: (hidden)" -ForegroundColor White
Write-Host "Database: $database" -ForegroundColor White
