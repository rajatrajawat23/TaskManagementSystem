#!/bin/bash

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# API Base URL
BASE_URL="http://localhost:5175/api"

# Function to print section headers
print_header() {
    echo -e "\n${BLUE}=== $1 ===${NC}"
}

# Function to test endpoint with detailed output
test_endpoint_debug() {
    local method=$1
    local endpoint=$2
    local data=$3
    local token=$4
    local description=$5
    
    echo -e "\n${YELLOW}Testing: $description${NC}"
    echo "Method: $method"
    echo "Endpoint: $BASE_URL$endpoint"
    if [ -n "$data" ]; then
        echo "Request Body:"
        echo "$data" | jq '.' 2>/dev/null || echo "$data"
    fi
    
    if [ -n "$token" ]; then
        response=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X $method \
            -H "Content-Type: application/json" \
            -H "Authorization: Bearer $token" \
            -d "$data" \
            "$BASE_URL$endpoint" 2>&1)
    else
        response=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X $method \
            -H "Content-Type: application/json" \
            -d "$data" \
            "$BASE_URL$endpoint" 2>&1)
    fi
    
    # Extract status code and body
    status_code=$(echo "$response" | grep -o "HTTP_STATUS:[0-9]*" | cut -d: -f2)
    body=$(echo "$response" | sed '$d')
    
    echo "Response Status: $status_code"
    echo "Response Body:"
    echo "$body" | jq '.' 2>/dev/null || echo "$body"
    
    if [[ "$status_code" =~ ^2[0-9][0-9]$ ]]; then
        echo -e "${GREEN}✅ Success${NC}"
        return 0
    else
        echo -e "${RED}❌ Failed${NC}"
        return 1
    fi
}

# Check if API is running
print_header "Checking API Health"
health_check=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5175/health)
if [ "$health_check" != "200" ]; then
    echo -e "${RED}API is not running! Starting it now...${NC}"
    cd Backend/TaskManagement.API
    screen -dmS taskapi dotnet run
    echo "Waiting for API to start..."
    sleep 10
fi

# 1. LOGIN TO GET TOKEN
print_header "1. Authentication"
echo "Attempting login..."
login_response=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -d '{"email":"superadmin@taskmanagement.com","password":"Admin@123"}' \
    "$BASE_URL/auth/login")

echo "Login Response:"
echo "$login_response" | jq '.'

if echo "$login_response" | jq -e '.accessToken' > /dev/null 2>&1; then
    TOKEN=$(echo "$login_response" | jq -r '.accessToken')
    USER_ID=$(echo "$login_response" | jq -r '.userId')
    echo -e "${GREEN}✅ Login successful${NC}"
    echo "Token: ${TOKEN:0:50}..."
    echo "User ID: $USER_ID"
else
    echo -e "${RED}❌ Login failed${NC}"
    exit 1
fi

# 2. TEST TASK CRUD OPERATIONS
print_header "2. Task CRUD Operations"

# Get current user's company
echo -e "\nGetting user's company..."
profile_response=$(curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/user/profile")
COMPANY_ID=$(echo "$profile_response" | jq -r '.companyId // empty')
echo "Company ID: $COMPANY_ID"

# Create a task
test_endpoint_debug "POST" "/task" \
    "{
        \"title\": \"Test Task $(date +%s)\",
        \"description\": \"This is a test task description\",
        \"priority\": \"High\",
        \"status\": \"Pending\",
        \"dueDate\": \"2025-07-01T00:00:00\",
        \"category\": \"Development\",
        \"estimatedHours\": 8
    }" \
    "$TOKEN" "Create New Task"

# If task creation succeeded, get the task ID from response
if [ $? -eq 0 ]; then
    TASK_ID=$(echo "$body" | jq -r '.id // empty')
    if [ -n "$TASK_ID" ]; then
        # Get task by ID
        test_endpoint_debug "GET" "/task/$TASK_ID" "" "$TOKEN" "Get Task by ID"
        
        # Update task
        test_endpoint_debug "PUT" "/task/$TASK_ID" \
            "{
                \"title\": \"Updated Test Task\",
                \"description\": \"Updated description\",
                \"priority\": \"Medium\",
                \"status\": \"InProgress\"
            }" \
            "$TOKEN" "Update Task"
        
        # Update task status
        test_endpoint_debug "PUT" "/task/$TASK_ID/status" \
            "{\"status\": \"Completed\"}" \
            "$TOKEN" "Update Task Status"
        
        # Delete task
        test_endpoint_debug "DELETE" "/task/$TASK_ID" "" "$TOKEN" "Delete Task"
    fi
fi

# 3. TEST CLIENT CRUD OPERATIONS
print_header "3. Client CRUD Operations"

# Create a client
test_endpoint_debug "POST" "/client" \
    "{
        \"name\": \"Test Client $(date +%s)\",
        \"email\": \"testclient@example.com\",
        \"phone\": \"+1234567890\",
        \"contactPerson\": \"John Doe\",
        \"industry\": \"Technology\",
        \"address\": \"123 Main St, City, Country\"
    }" \
    "$TOKEN" "Create New Client"

# 4. TEST PROJECT CRUD OPERATIONS
print_header "4. Project CRUD Operations"

# First, get a client ID for the project
clients_response=$(curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/client?pageSize=1")
CLIENT_ID=$(echo "$clients_response" | jq -r '.items[0].id // empty')

# Create a project
test_endpoint_debug "POST" "/project" \
    "{
        \"name\": \"Test Project $(date +%s)\",
        \"description\": \"Test project description\",
        \"projectCode\": \"PROJ-$(date +%s)\",
        \"startDate\": \"2025-06-18T00:00:00\",
        \"endDate\": \"2025-12-31T00:00:00\",
        \"status\": \"Planning\",
        \"budget\": 100000.00,
        \"clientId\": $([ -n "$CLIENT_ID" ] && echo "\"$CLIENT_ID\"" || echo "null")
    }" \
    "$TOKEN" "Create New Project"

# 5. TEST USER CRUD OPERATIONS (Admin function)
print_header "5. User CRUD Operations"

# Create a user
test_endpoint_debug "POST" "/user" \
    "{
        \"email\": \"testuser$(date +%s)@example.com\",
        \"password\": \"User@123\",
        \"firstName\": \"Test\",
        \"lastName\": \"User\",
        \"role\": \"User\",
        \"companyId\": \"$COMPANY_ID\"
    }" \
    "$TOKEN" "Create New User"

# 6. CHECK DATABASE DIRECTLY
print_header "6. Database Verification"

echo "Checking database records..."
docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P 'SecureTask2025#@!' -d TaskManagementDB -C \
    -Q "SELECT TOP 5 'Tasks' as Entity, Id, Title, Status, CreatedAt FROM [Core].[Tasks] WHERE IsDeleted = 0 ORDER BY CreatedAt DESC
         UNION ALL
         SELECT TOP 5 'Clients', CAST(Id as nvarchar(50)), Name, 'Active', CreatedAt FROM [Core].[Clients] WHERE IsDeleted = 0 ORDER BY CreatedAt DESC
         UNION ALL
         SELECT TOP 5 'Projects', CAST(Id as nvarchar(50)), Name, Status, CreatedAt FROM [Core].[Projects] WHERE IsDeleted = 0 ORDER BY CreatedAt DESC" \
    -s "|" -W 2>/dev/null || echo "Database query failed"

print_header "Test Complete"
echo -e "${YELLOW}Note: Check the detailed output above to identify specific issues with CRUD operations.${NC}"
