#!/bin/bash

# 🔍 Comprehensive Backend Check Script for TaskManagementSystem
# This script will verify:
# 1. All controllers and their endpoints
# 2. All services and their methods
# 3. Database tables and entities
# 4. CRUD operations for each entity
# 5. API parameters and request/response models

API_BASE_URL="http://localhost:5000/api"
RESULTS_FILE="backend-verification-report.md"

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# JWT Token (will be obtained after login)
JWT_TOKEN=""

# Initialize report
cat > "$RESULTS_FILE" << EOF
# 🔍 Backend Verification Report - TaskManagementSystem
Generated on: $(date)

## Summary
This report provides a comprehensive analysis of all backend components including:
- Controllers and their endpoints
- Services and their methods
- Database entities and tables
- CRUD operations testing
- API parameters and payloads

---

EOF

# Function to print section headers
print_section() {
    echo -e "\n${BLUE}========================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}========================================${NC}"
    echo -e "\n## $1\n" >> "$RESULTS_FILE"
}

# Function to check API health
check_api_health() {
    print_section "API Health Check"
    
    echo "Checking API health..."
    HEALTH_CHECK=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/health)
    
    if [ "$HEALTH_CHECK" == "200" ]; then
        echo -e "${GREEN}✅ API is running and healthy${NC}"
        echo "✅ API is running and healthy" >> "$RESULTS_FILE"
    else
        echo -e "${RED}❌ API is not responding (HTTP $HEALTH_CHECK)${NC}"
        echo "❌ API is not responding (HTTP $HEALTH_CHECK)" >> "$RESULTS_FILE"
        exit 1
    fi
}

# Function to authenticate and get JWT token
authenticate() {
    print_section "Authentication Test"
    
    echo "Attempting login..."
    LOGIN_RESPONSE=$(curl -s -X POST "$API_BASE_URL/auth/login" \
        -H "Content-Type: application/json" \
        -d '{
            "email": "superadmin@taskmanagement.com",
            "password": "SuperAdmin123!"
        }')
    
    JWT_TOKEN=$(echo $LOGIN_RESPONSE | jq -r '.token // empty')
    
    if [ -n "$JWT_TOKEN" ]; then
        echo -e "${GREEN}✅ Authentication successful${NC}"
        echo "✅ Authentication successful" >> "$RESULTS_FILE"
        echo "Token obtained: ${JWT_TOKEN:0:20}..." >> "$RESULTS_FILE"
    else
        echo -e "${RED}❌ Authentication failed${NC}"
        echo "❌ Authentication failed" >> "$RESULTS_FILE"
        echo "Response: $LOGIN_RESPONSE" >> "$RESULTS_FILE"
    fi
}

# Function to analyze controllers
analyze_controllers() {
    print_section "Controllers Analysis"
    
    echo "### Available Controllers:" >> "$RESULTS_FILE"
    echo "" >> "$RESULTS_FILE"
    
    # List all controllers
    for controller in /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Controllers/*.cs; do
        if [ -f "$controller" ]; then
            controller_name=$(basename "$controller" .cs)
            echo -e "${YELLOW}Analyzing $controller_name...${NC}"
            echo "#### $controller_name" >> "$RESULTS_FILE"
            
            # Extract endpoints from controller
            echo "Endpoints:" >> "$RESULTS_FILE"
            grep -E '\[Http(Get|Post|Put|Delete|Patch)\]|^\s*public\s+.*\s+\w+\s*\(' "$controller" | while read -r line; do
                if [[ $line =~ \[Http ]]; then
                    echo "  - $line" >> "$RESULTS_FILE"
                fi
            done
            echo "" >> "$RESULTS_FILE"
        fi
    done
}

# Function to analyze services
analyze_services() {
    print_section "Services Analysis"
    
    echo "### Available Services:" >> "$RESULTS_FILE"
    echo "" >> "$RESULTS_FILE"
    
    # List all services
    for service in /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Services/Implementation/*.cs; do
        if [ -f "$service" ]; then
            service_name=$(basename "$service" .cs)
            if [[ ! "$service_name" =~ \.backup$ ]]; then
                echo -e "${YELLOW}Analyzing $service_name...${NC}"
                echo "#### $service_name" >> "$RESULTS_FILE"
                
                # Extract public methods from service
                echo "Methods:" >> "$RESULTS_FILE"
                grep -E '^\s*public\s+(async\s+)?Task<.*>\s+\w+\s*\(|^\s*public\s+.*\s+\w+\s*\(' "$service" | while read -r line; do
                    # Clean up the method signature
                    method_sig=$(echo "$line" | sed 's/^\s*//' | sed 's/{$//')
                    echo "  - $method_sig" >> "$RESULTS_FILE"
                done
                echo "" >> "$RESULTS_FILE"
            fi
        fi
    done
}

# Function to check database entities
check_database_entities() {
    print_section "Database Entities"
    
    echo "### Core Entities:" >> "$RESULTS_FILE"
    echo "" >> "$RESULTS_FILE"
    
    # List all entities
    for entity in /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.Core/Entities/*.cs; do
        if [ -f "$entity" ]; then
            entity_name=$(basename "$entity" .cs)
            echo -e "${YELLOW}Entity: $entity_name${NC}"
            echo "- $entity_name" >> "$RESULTS_FILE"
        fi
    done
}

# Function to test CRUD operations for an entity
test_crud_operations() {
    local entity_name=$1
    local endpoint=$2
    local create_payload=$3
    local update_payload=$4
    
    echo -e "\n${YELLOW}Testing CRUD for $entity_name${NC}"
    echo "### $entity_name CRUD Test" >> "$RESULTS_FILE"
    
    # Test GET all
    echo -n "GET all $endpoint: "
    GET_ALL_RESPONSE=$(curl -s -X GET "$API_BASE_URL/$endpoint" \
        -H "Authorization: Bearer $JWT_TOKEN" \
        -w "\n%{http_code}")
    
    HTTP_CODE=$(echo "$GET_ALL_RESPONSE" | tail -n 1)
    RESPONSE_BODY=$(echo "$GET_ALL_RESPONSE" | head -n -1)
    
    if [ "$HTTP_CODE" == "200" ]; then
        echo -e "${GREEN}✅ Success${NC}"
        echo "- GET all: ✅ Success (200)" >> "$RESULTS_FILE"
    else
        echo -e "${RED}❌ Failed (HTTP $HTTP_CODE)${NC}"
        echo "- GET all: ❌ Failed (HTTP $HTTP_CODE)" >> "$RESULTS_FILE"
    fi
    
    # Test CREATE
    if [ -n "$create_payload" ]; then
        echo -n "POST create $endpoint: "
        CREATE_RESPONSE=$(curl -s -X POST "$API_BASE_URL/$endpoint" \
            -H "Authorization: Bearer $JWT_TOKEN" \
            -H "Content-Type: application/json" \
            -d "$create_payload" \
            -w "\n%{http_code}")
        
        HTTP_CODE=$(echo "$CREATE_RESPONSE" | tail -n 1)
        RESPONSE_BODY=$(echo "$CREATE_RESPONSE" | head -n -1)
        
        if [[ "$HTTP_CODE" == "200" || "$HTTP_CODE" == "201" ]]; then
            echo -e "${GREEN}✅ Success${NC}"
            echo "- POST create: ✅ Success ($HTTP_CODE)" >> "$RESULTS_FILE"
            
            # Extract ID for further operations
            CREATED_ID=$(echo "$RESPONSE_BODY" | jq -r '.id // .data.id // empty')
            
            if [ -n "$CREATED_ID" ]; then
                # Test GET by ID
                echo -n "GET by ID $endpoint/$CREATED_ID: "
                GET_BY_ID_RESPONSE=$(curl -s -X GET "$API_BASE_URL/$endpoint/$CREATED_ID" \
                    -H "Authorization: Bearer $JWT_TOKEN" \
                    -w "\n%{http_code}")
                
                HTTP_CODE=$(echo "$GET_BY_ID_RESPONSE" | tail -n 1)
                
                if [ "$HTTP_CODE" == "200" ]; then
                    echo -e "${GREEN}✅ Success${NC}"
                    echo "- GET by ID: ✅ Success (200)" >> "$RESULTS_FILE"
                else
                    echo -e "${RED}❌ Failed (HTTP $HTTP_CODE)${NC}"
                    echo "- GET by ID: ❌ Failed (HTTP $HTTP_CODE)" >> "$RESULTS_FILE"
                fi
                
                # Test UPDATE
                if [ -n "$update_payload" ]; then
                    echo -n "PUT update $endpoint/$CREATED_ID: "
                    UPDATE_RESPONSE=$(curl -s -X PUT "$API_BASE_URL/$endpoint/$CREATED_ID" \
                        -H "Authorization: Bearer $JWT_TOKEN" \
                        -H "Content-Type: application/json" \
                        -d "$update_payload" \
                        -w "\n%{http_code}")
                    
                    HTTP_CODE=$(echo "$UPDATE_RESPONSE" | tail -n 1)
                    
                    if [ "$HTTP_CODE" == "200" ]; then
                        echo -e "${GREEN}✅ Success${NC}"
                        echo "- PUT update: ✅ Success (200)" >> "$RESULTS_FILE"
                    else
                        echo -e "${RED}❌ Failed (HTTP $HTTP_CODE)${NC}"
                        echo "- PUT update: ❌ Failed (HTTP $HTTP_CODE)" >> "$RESULTS_FILE"
                    fi
                fi
                
                # Test DELETE
                echo -n "DELETE $endpoint/$CREATED_ID: "
                DELETE_RESPONSE=$(curl -s -X DELETE "$API_BASE_URL/$endpoint/$CREATED_ID" \
                    -H "Authorization: Bearer $JWT_TOKEN" \
                    -w "\n%{http_code}")
                
                HTTP_CODE=$(echo "$DELETE_RESPONSE" | tail -n 1)
                
                if [[ "$HTTP_CODE" == "200" || "$HTTP_CODE" == "204" ]]; then
                    echo -e "${GREEN}✅ Success${NC}"
                    echo "- DELETE: ✅ Success ($HTTP_CODE)" >> "$RESULTS_FILE"
                else
                    echo -e "${RED}❌ Failed (HTTP $HTTP_CODE)${NC}"
                    echo "- DELETE: ❌ Failed (HTTP $HTTP_CODE)" >> "$RESULTS_FILE"
                fi
            fi
        else
            echo -e "${RED}❌ Failed (HTTP $HTTP_CODE)${NC}"
            echo "- POST create: ❌ Failed (HTTP $HTTP_CODE)" >> "$RESULTS_FILE"
            echo "Response: $RESPONSE_BODY" >> "$RESULTS_FILE"
        fi
    fi
    
    echo "" >> "$RESULTS_FILE"
}

# Function to analyze service parameters
analyze_service_parameters() {
    print_section "Service Method Parameters Analysis"
    
    echo "### Service Methods and Their Parameters:" >> "$RESULTS_FILE"
    echo "" >> "$RESULTS_FILE"
    
    # Analyze each service file
    for service in /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Services/Implementation/*.cs; do
        if [ -f "$service" ]; then
            service_name=$(basename "$service" .cs)
            if [[ ! "$service_name" =~ \.backup$ ]]; then
                echo "#### $service_name" >> "$RESULTS_FILE"
                echo "" >> "$RESULTS_FILE"
                
                # Extract method signatures with parameters
                grep -A 5 -E '^\s*public\s+(async\s+)?Task<.*>\s+\w+\s*\(|^\s*public\s+.*\s+\w+\s*\(' "$service" | while IFS= read -r line; do
                    if [[ $line =~ public ]]; then
                        echo "**Method:** $line" >> "$RESULTS_FILE"
                    elif [[ $line =~ "var\s+entity\s*=|_context\.|_dbContext\." ]]; then
                        echo "  - Database operation: $line" >> "$RESULTS_FILE"
                    fi
                done
                echo "" >> "$RESULTS_FILE"
            fi
        fi
    done
}

# Main execution
main() {
    echo -e "${GREEN}🚀 Starting Comprehensive Backend Check...${NC}"
    
    # Check if API is running
    check_api_health
    
    # Authenticate
    authenticate
    
    # Analyze controllers
    analyze_controllers
    
    # Analyze services
    analyze_services
    
    # Check database entities
    check_database_entities
    
    # Test CRUD operations for each major entity
    print_section "CRUD Operations Testing"
    
    # Test Task CRUD
    test_crud_operations "Task" "task" \
        '{"title":"Test Task","description":"Test Description","priority":"High","status":"Pending","dueDate":"2025-06-20T00:00:00"}' \
        '{"title":"Updated Task","description":"Updated Description","priority":"Medium"}'
    
    # Test User CRUD
    test_crud_operations "User" "user" \
        '{"email":"testuser@test.com","password":"Test123!","firstName":"Test","lastName":"User","role":"User"}' \
        '{"firstName":"Updated","lastName":"User"}'
    
    # Test Client CRUD
    test_crud_operations "Client" "client" \
        '{"name":"Test Client","email":"client@test.com","phone":"1234567890","address":"Test Address"}' \
        '{"name":"Updated Client","email":"updated@test.com"}'
    
    # Test Project CRUD
    test_crud_operations "Project" "project" \
        '{"name":"Test Project","description":"Test Description","status":"Active","startDate":"2025-06-18T00:00:00","endDate":"2025-12-31T00:00:00"}' \
        '{"name":"Updated Project","status":"InProgress"}'
    
    # Test Company CRUD
    test_crud_operations "Company" "company" \
        '{"name":"Test Company","domain":"testcompany","contactEmail":"admin@testcompany.com","subscriptionType":"Premium"}' \
        '{"name":"Updated Company","contactEmail":"updated@testcompany.com"}'
    
    # Analyze service parameters
    analyze_service_parameters
    
    echo -e "\n${GREEN}✅ Backend verification complete!${NC}"
    echo -e "${BLUE}📊 Report saved to: $RESULTS_FILE${NC}"
    
    # Show summary
    echo -e "\n${BLUE}Summary:${NC}"
    echo "Controllers found: $(ls /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Controllers/*.cs 2>/dev/null | wc -l)"
    echo "Services found: $(ls /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Services/Implementation/*.cs 2>/dev/null | grep -v backup | wc -l)"
    echo "Entities found: $(ls /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.Core/Entities/*.cs 2>/dev/null | wc -l)"
}

# Run main function
main
