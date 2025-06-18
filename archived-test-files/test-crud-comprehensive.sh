#!/bin/bash

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

BASE_URL="http://localhost:5175/api"
LOG_FILE="crud-test-results.log"

# Initialize log
echo "CRUD Operations Test - $(date)" > $LOG_FILE

# Function to log and display
log_and_display() {
    echo -e "$1"
    echo "$1" | sed 's/\x1b\[[0-9;]*m//g' >> $LOG_FILE
}

# Function to test endpoint
test_endpoint() {
    local method=$1
    local endpoint=$2
    local data=$3
    local token=$4
    local expected=$5
    local description=$6
    
    log_and_display "\n${YELLOW}Testing: $description${NC}"
    
    # Log request details
    echo "Method: $method" >> $LOG_FILE
    echo "Endpoint: $BASE_URL$endpoint" >> $LOG_FILE
    if [ -n "$data" ]; then
        echo "Request Body:" >> $LOG_FILE
        echo "$data" >> $LOG_FILE
    fi
    
    # Make request
    if [ -n "$token" ]; then
        if [ -n "$data" ]; then
            response=$(curl -s -w "\nSTATUS_CODE:%{http_code}" -X $method \
                -H "Content-Type: application/json" \
                -H "Authorization: Bearer $token" \
                -d "$data" \
                "$BASE_URL$endpoint" 2>&1)
        else
            response=$(curl -s -w "\nSTATUS_CODE:%{http_code}" -X $method \
                -H "Authorization: Bearer $token" \
                "$BASE_URL$endpoint" 2>&1)
        fi
    else
        response=$(curl -s -w "\nSTATUS_CODE:%{http_code}" -X $method \
            -H "Content-Type: application/json" \
            -d "$data" \
            "$BASE_URL$endpoint" 2>&1)
    fi
    
    # Extract status and body
    status_code=$(echo "$response" | grep -o "STATUS_CODE:[0-9]*" | cut -d: -f2)
    body=$(echo "$response" | sed '/STATUS_CODE:/d')
    
    # Log response
    echo "Response Status: $status_code" >> $LOG_FILE
    echo "Response Body:" >> $LOG_FILE
    echo "$body" >> $LOG_FILE
    echo "---" >> $LOG_FILE
    
    # Display result
    if [[ "$status_code" == "$expected" ]]; then
        log_and_display "${GREEN}✅ Success (Status: $status_code)${NC}"
        echo "$body" | jq '.' 2>/dev/null || echo "$body"
        # Return the body for further processing
        echo "RETURN:$body"
        return 0
    else
        log_and_display "${RED}❌ Failed (Expected: $expected, Got: $status_code)${NC}"
        echo "$body" | jq '.' 2>/dev/null || echo "$body"
        return 1
    fi
}

# Start tests
log_and_display "${BLUE}=== Comprehensive CRUD Test Suite ===${NC}"

# 1. LOGIN
log_and_display "\n${BLUE}1. Authentication${NC}"
auth_result=$(test_endpoint "POST" "/auth/login" \
    '{"email":"superadmin@taskmanagement.com","password":"Admin@123"}' \
    "" "200" "Login as SuperAdmin")

TOKEN=$(echo "$auth_result" | grep "RETURN:" | cut -d: -f2- | jq -r '.accessToken // empty')
if [ -z "$TOKEN" ]; then
    log_and_display "${RED}Failed to get auth token. Exiting.${NC}"
    exit 1
fi

# 2. CREATE COMPANY WITH ADMIN
log_and_display "\n${BLUE}2. Company Management${NC}"

# Check existing companies first
existing_companies=$(curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/company")
COMPANY_COUNT=$(echo "$existing_companies" | jq '.totalCount // 0')
log_and_display "Existing companies: $COMPANY_COUNT"

# Create a new company with admin details
TIMESTAMP=$(date +%s)
company_result=$(test_endpoint "POST" "/company" \
    "{
        \"name\": \"Test Company $TIMESTAMP\",
        \"domain\": \"testcompany$TIMESTAMP\",
        \"contactEmail\": \"contact@testcompany$TIMESTAMP.com\",
        \"contactPhone\": \"+1234567890\",
        \"address\": \"123 Test Street, Test City\",
        \"subscriptionType\": \"Premium\",
        \"maxUsers\": 50,
        \"adminEmail\": \"admin$TIMESTAMP@testcompany.com\",
        \"adminPassword\": \"Admin@123\",
        \"adminFirstName\": \"Admin\",
        \"adminLastName\": \"User\"
    }" \
    "$TOKEN" "201" "Create Company with Admin")

COMPANY_ID=$(echo "$company_result" | grep "RETURN:" | cut -d: -f2- | jq -r '.id // empty')
ADMIN_ID=$(echo "$company_result" | grep "RETURN:" | cut -d: -f2- | jq -r '.adminUserId // empty')

if [ -z "$COMPANY_ID" ]; then
    # Use existing company
    COMPANY_ID=$(echo "$existing_companies" | jq -r '.items[0].id // empty')
    log_and_display "Using existing company: $COMPANY_ID"
fi

# 3. CREATE USERS
log_and_display "\n${BLUE}3. User Management${NC}"

# Create Manager
manager_result=$(test_endpoint "POST" "/user" \
    "{
        \"email\": \"manager$TIMESTAMP@testcompany.com\",
        \"password\": \"Manager@123\",
        \"firstName\": \"Test\",
        \"lastName\": \"Manager\",
        \"role\": \"Manager\",
        \"companyId\": \"$COMPANY_ID\",
        \"department\": \"Operations\",
        \"jobTitle\": \"Operations Manager\"
    }" \
    "$TOKEN" "201" "Create Manager User")

MANAGER_ID=$(echo "$manager_result" | grep "RETURN:" | cut -d: -f2- | jq -r '.id // empty')

# Create Regular User
user_result=$(test_endpoint "POST" "/user" \
    "{
        \"email\": \"user$TIMESTAMP@testcompany.com\",
        \"password\": \"User@123\",
        \"firstName\": \"Test\",
        \"lastName\": \"User\",
        \"role\": \"User\",
        \"companyId\": \"$COMPANY_ID\",
        \"department\": \"Development\",
        \"jobTitle\": \"Developer\"
    }" \
    "$TOKEN" "201" "Create Regular User")

USER_ID=$(echo "$user_result" | grep "RETURN:" | cut -d: -f2- | jq -r '.id // empty')

# 4. CLIENT CRUD
log_and_display "\n${BLUE}4. Client Management${NC}"

# Create Client
client_result=$(test_endpoint "POST" "/client" \
    "{
        \"name\": \"Test Client $TIMESTAMP\",
        \"email\": \"client$TIMESTAMP@example.com\",
        \"phone\": \"+1234567890\",
        \"contactPerson\": \"John Doe\",
        \"address\": \"456 Client Street\",
        \"city\": \"Client City\",
        \"country\": \"USA\",
        \"industry\": \"Technology\",
        \"website\": \"https://testclient$TIMESTAMP.com\",
        \"notes\": \"Important client\"
    }" \
    "$TOKEN" "201" "Create Client")

CLIENT_ID=$(echo "$client_result" | grep "RETURN:" | cut -d: -f2- | jq -r '.id // empty')

if [ -n "$CLIENT_ID" ]; then
    # Read Client
    test_endpoint "GET" "/client/$CLIENT_ID" "" "$TOKEN" "200" "Get Client by ID"
    
    # Update Client
    test_endpoint "PUT" "/client/$CLIENT_ID" \
        "{
            \"name\": \"Updated Client Name\",
            \"email\": \"updated$TIMESTAMP@example.com\",
            \"contactPerson\": \"Jane Doe\"
        }" \
        "$TOKEN" "200" "Update Client"
fi

# 5. PROJECT CRUD
log_and_display "\n${BLUE}5. Project Management${NC}"

# Create Project
project_result=$(test_endpoint "POST" "/project" \
    "{
        \"name\": \"Test Project $TIMESTAMP\",
        \"description\": \"This is a test project\",
        \"projectCode\": \"PROJ-$TIMESTAMP\",
        \"startDate\": \"2025-06-18T00:00:00Z\",
        \"endDate\": \"2025-12-31T00:00:00Z\",
        \"status\": \"Planning\",
        \"budget\": 100000,
        \"projectManagerId\": \"${MANAGER_ID:-$ADMIN_ID}\",
        \"clientId\": $([ -n "$CLIENT_ID" ] && echo "\"$CLIENT_ID\"" || echo "null")
    }" \
    "$TOKEN" "201" "Create Project")

PROJECT_ID=$(echo "$project_result" | grep "RETURN:" | cut -d: -f2- | jq -r '.id // empty')

if [ -n "$PROJECT_ID" ]; then
    # Read Project
    test_endpoint "GET" "/project/$PROJECT_ID" "" "$TOKEN" "200" "Get Project by ID"
    
    # Update Project
    test_endpoint "PUT" "/project/$PROJECT_ID" \
        "{
            \"name\": \"Updated Project Name\",
            \"status\": \"InProgress\",
            \"progress\": 25
        }" \
        "$TOKEN" "200" "Update Project"
fi

# 6. TASK CRUD
log_and_display "\n${BLUE}6. Task Management${NC}"

# Create Task
task_result=$(test_endpoint "POST" "/task" \
    "{
        \"title\": \"Test Task $TIMESTAMP\",
        \"description\": \"This is a test task\",
        \"priority\": \"High\",
        \"status\": \"Pending\",
        \"category\": \"Development\",
        \"estimatedHours\": 8,
        \"dueDate\": \"2025-07-01T00:00:00Z\",
        \"assignedToId\": $([ -n "$USER_ID" ] && echo "\"$USER_ID\"" || echo "null"),
        \"projectId\": $([ -n "$PROJECT_ID" ] && echo "\"$PROJECT_ID\"" || echo "null"),
        \"clientId\": $([ -n "$CLIENT_ID" ] && echo "\"$CLIENT_ID\"" || echo "null"),
        \"companyId\": \"$COMPANY_ID\"
    }" \
    "$TOKEN" "201" "Create Task")

TASK_ID=$(echo "$task_result" | grep "RETURN:" | cut -d: -f2- | jq -r '.id // empty')

if [ -n "$TASK_ID" ]; then
    # Read Task
    test_endpoint "GET" "/task/$TASK_ID" "" "$TOKEN" "200" "Get Task by ID"
    
    # Update Task
    test_endpoint "PUT" "/task/$TASK_ID" \
        "{
            \"title\": \"Updated Task Title\",
            \"description\": \"Updated description\",
            \"priority\": \"Medium\",
            \"status\": \"InProgress\",
            \"progress\": 50
        }" \
        "$TOKEN" "200" "Update Task"
    
    # Update Task Status
    test_endpoint "PUT" "/task/$TASK_ID/status" \
        "{\"status\": \"Completed\"}" \
        "$TOKEN" "200" "Update Task Status"
    
    # Delete Task (soft delete)
    test_endpoint "DELETE" "/task/$TASK_ID" "" "$TOKEN" "200" "Delete Task"
fi

# 7. SUMMARY
log_and_display "\n${BLUE}7. Test Summary${NC}"

# Count records in database
log_and_display "\n${YELLOW}Database Record Count:${NC}"
docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P 'SecureTask2025#@!' -d TaskManagementDB -C \
    -Q "SELECT 'Companies' as Entity, COUNT(*) as Count FROM [Core].[Companies] WHERE IsDeleted = 0
         UNION ALL SELECT 'Users', COUNT(*) FROM [Security].[Users] WHERE IsDeleted = 0
         UNION ALL SELECT 'Clients', COUNT(*) FROM [Core].[Clients] WHERE IsDeleted = 0
         UNION ALL SELECT 'Projects', COUNT(*) FROM [Core].[Projects] WHERE IsDeleted = 0
         UNION ALL SELECT 'Tasks', COUNT(*) FROM [Core].[Tasks] WHERE IsDeleted = 0" \
    -s "|" -W 2>/dev/null

# Count successes and failures
SUCCESS_COUNT=$(grep -c "✅" $LOG_FILE)
FAILURE_COUNT=$(grep -c "❌" $LOG_FILE)

log_and_display "\n${BLUE}=== Final Results ===${NC}"
log_and_display "${GREEN}Successful operations: $SUCCESS_COUNT${NC}"
log_and_display "${RED}Failed operations: $FAILURE_COUNT${NC}"
log_and_display "\nDetailed log saved to: $LOG_FILE"

# Show specific errors if any
if [ $FAILURE_COUNT -gt 0 ]; then
    log_and_display "\n${YELLOW}Failed Operations Summary:${NC}"
    grep -B2 "❌" $LOG_FILE | grep -E "Testing:|❌" | sed 's/Testing: /  - /'
fi
