#!/bin/bash

# Database setup script
echo "=== Setting up TaskManagement Database ==="

# SQL Server connection details
SQL_SERVER="localhost"
SQL_USER="sa"
SQL_PASSWORD="SecureTask2025#@!"

# Function to execute SQL file
execute_sql() {
    local file=$1
    echo "Executing: $file"
    docker exec -i taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd \
        -S $SQL_SERVER -U $SQL_USER -P "$SQL_PASSWORD" -C \
        -i /dev/stdin < $file
    
    if [ $? -eq 0 ]; then
        echo "✅ $file executed successfully"
    else
        echo "❌ Failed to execute $file"
        return 1
    fi
}

# Change to scripts directory
cd Database/Scripts

# Execute scripts in order
echo "1. Creating database..."
execute_sql "01-CreateDatabase.sql"

echo -e "\n2. Creating tables..."
execute_sql "02-CreateTables.sql"
execute_sql "02-CreateTables-Part2.sql"
execute_sql "02-CreateTables-Part3.sql"

echo -e "\n3. Creating indexes..."
execute_sql "03-CreateIndexes-Fixed.sql"

echo -e "\n4. Adding IsDeleted column..."
execute_sql "07-AddIsDeletedColumn.sql"

echo -e "\n5. Seeding data..."
execute_sql "05-SeedData-BCrypt.sql"

echo -e "\n6. Adding missing indexes and relations..."
execute_sql "08-AddMissingIndexesAndRelations.sql"

# Verify tables were created
echo -e "\n=== Verifying Database Setup ==="
docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S $SQL_SERVER -U $SQL_USER -P "$SQL_PASSWORD" -d TaskManagementDB -C \
    -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME" \
    -h -1

echo -e "\n=== Checking record counts ==="
docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S $SQL_SERVER -U $SQL_USER -P "$SQL_PASSWORD" -d TaskManagementDB -C \
    -Q "SELECT 'Companies' as TableName, COUNT(*) as Count FROM Companies
         UNION ALL SELECT 'Users', COUNT(*) FROM Users
         UNION ALL SELECT 'Tasks', COUNT(*) FROM Tasks
         UNION ALL SELECT 'Projects', COUNT(*) FROM Projects
         UNION ALL SELECT 'Clients', COUNT(*) FROM Clients" \
    -h -1

echo -e "\n=== Database setup complete! ==="
