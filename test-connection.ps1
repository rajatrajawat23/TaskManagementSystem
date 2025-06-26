# Test SQL Server connection
Write-Host "Testing connection to SQL Server..." -ForegroundColor Cyan

# Connection details from your connection string
$server = "192.168.1.101,1433"
$user = "sa"
$password = "De$ktop"

try {
    # Simple test query
    $result = Invoke-Expression "sqlcmd -S `"$server`" -U `"$user`" -P `"$password`" -Q `"SELECT @@VERSION`" -l 30"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Connection successful!" -ForegroundColor Green
        Write-Host "SQL Server Version:" -ForegroundColor Cyan
        Write-Host $result
    } else {
        Write-Host "Connection failed with exit code $LASTEXITCODE" -ForegroundColor Red
    }
} catch {
    Write-Host "Error connecting to SQL Server: $_" -ForegroundColor Red
}

Write-Host "`nPlease verify your connection details:" -ForegroundColor Yellow
Write-Host "Server: $server" -ForegroundColor White
Write-Host "User: $user" -ForegroundColor White
Write-Host "Password: (hidden for security)" -ForegroundColor White
