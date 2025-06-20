#!/bin/bash

# Project API Testing Script
# This script tests all Project API endpoints systematically

BASE_URL="http://localhost:5175"
TOKEN=$(cat company_admin_token_new.txt)
COMPANY_ID=$(cat company_id.txt)

echo "=========================================="
echo "🚀 PROJECT API TESTING STARTED"
echo "=========================================="
echo "Base URL: $BASE_URL"
echo "Company ID: $COMPANY_ID"
echo "Token: ${TOKEN:0:50}..."
echo "=========================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to test API endpoint
test_api() {
    local method=$1
    local endpoint=$2
    local data=$3
    local description=$4
    
    echo -e "\n${BLUE}📋 Testing: $description${NC}"
    echo -e "${YELLOW}$method $endpoint${NC}"
    
    if [ -n "$data" ]; then
        response=$(curl -s -w "\n%{http_code}" -X $method "$BASE_URL$endpoint" \
            -H "Authorization: Bearer $TOKEN" \
            -H "Content-Type: application/json" \
            -d "$data")
    else
        response=$(curl -s -w "\n%{http_code}" -X $method "$BASE_URL$endpoint" \
            -H "Authorization: Bearer $TOKEN")
    fi
    
    # Extract HTTP status code (last line)
    http_code=$(echo "$response" | tail -n1)
    # Extract response body (all lines except last)
    body=$(echo "$response" | head -n -1)
    
    echo "Status Code: $http_code"
    echo "Response:"
    echo "$body" | jq . 2>/dev/null || echo "$body"
    
    if [[ $http_code -ge 200 && $http_code -lt 300 ]]; then
        echo -e "${GREEN}✅ SUCCESS${NC}"
    else
        echo -e "${RED}❌ FAILED${NC}"
    fi
    
    # Save project ID if this was a create operation
    if [[ $endpoint == "/api/Project" && $method == "POST" && $http_code -eq 201 ]]; then
        PROJECT_ID=$(echo "$body" | jq -r '.id' 2>/dev/null)
        echo "PROJECT_ID=$PROJECT_ID" > project_id.txt
        echo "Project ID saved: $PROJECT_ID"
    fi
    
    echo "=========================================="
}

# 1. GET /api/Project - Get all projects
test_api "GET" "/api/Project" "" "Get All Projects (Empty List)"

# 2. POST /api/Project - Create new project
create_project_data='{
  "name": "Test Project Alpha",
  "description": "This is a test project for API testing purposes",
  "startDate": "2025-06-20T09:00:00Z",
  "endDate": "2025-12-20T17:00:00Z",
  "budget": 50000.00,
  "status": "Active",
  "projectManagerId": null
}'

test_api "POST" "/api/Project" "$create_project_data" "Create New Project"

# Check if project was created and get the ID
if [ -f "project_id.txt" ]; then
    source project_id.txt
    echo "Using Project ID: $PROJECT_ID"
    
    # 3. GET /api/Project/{id} - Get project by ID
    test_api "GET" "/api/Project/$PROJECT_ID" "" "Get Project by ID"
    
    # 4. PUT /api/Project/{id} - Update project
    update_project_data='{
      "id": "'$PROJECT_ID'",
      "name": "Updated Test Project Alpha",
      "description": "This project has been updated via API testing",
      "startDate": "2025-06-20T09:00:00Z",
      "endDate": "2025-12-31T17:00:00Z",
      "budget": 75000.00,
      "status": "Active",
      "projectManagerId": null
    }'
    
    test_api "PUT" "/api/Project/$PROJECT_ID" "$update_project_data" "Update Project"
    
    # 5. GET /api/Project/{id}/tasks - Get project tasks
    test_api "GET" "/api/Project/$PROJECT_ID/tasks" "" "Get Project Tasks"
    
    # 6. POST /api/Project/{id}/team-member - Add team member (using company admin user)
    USER_ID="2d19a358-ebf1-48d1-a4c1-eec6a934a499" # Company admin user ID from registration
    add_member_data='{
      "userId": "'$USER_ID'",
      "role": "Member"
    }'
    
    test_api "POST" "/api/Project/$PROJECT_ID/team-member" "$add_member_data" "Add Team Member"
    
    # 7. GET /api/Project/{id}/statistics - Get project statistics
    test_api "GET" "/api/Project/$PROJECT_ID/statistics" "" "Get Project Statistics"
    
    # 8. DELETE /api/Project/{id}/team-member/{userId} - Remove team member
    test_api "DELETE" "/api/Project/$PROJECT_ID/team-member/$USER_ID" "" "Remove Team Member"
    
    # 9. DELETE /api/Project/{id} - Delete project (soft delete)
    test_api "DELETE" "/api/Project/$PROJECT_ID" "" "Delete Project (Soft Delete)"
    
    # 10. Verify project is deleted by trying to get it
    test_api "GET" "/api/Project/$PROJECT_ID" "" "Verify Project Deletion"
    
else
    echo -e "${RED}❌ Could not find project ID. Skipping ID-based tests.${NC}"
fi

# Final: GET /api/Project - Get all projects again to see final state
test_api "GET" "/api/Project" "" "Get All Projects (Final State)"

echo -e "\n${GREEN}🎉 PROJECT API TESTING COMPLETED!${NC}"
echo "=========================================="
echo "📊 SUMMARY:"
echo "✅ All Project API endpoints have been tested"
echo "🗃️  Database verification needed:"
echo "   - Check if projects are saved in [Core].[Projects] table"
echo "   - Verify soft delete functionality (IsDeleted = 1)"
echo "   - Confirm project-user relationships in team member operations"
echo "=========================================="