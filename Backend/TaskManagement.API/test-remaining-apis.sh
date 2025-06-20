#!/bin/bash

# Set base URL and get JWT tokens
BASE_URL="http://localhost:5175"
SUPER_ADMIN_TOKEN=$(curl -s -X POST "$BASE_URL/api/Auth/login" -H "Content-Type: application/json" -d '{"email": "superadmin@taskmanagement.com", "password": "Admin@123"}' | jq -r '.accessToken')
COMPANY_ADMIN_TOKEN=$(curl -s -X POST "$BASE_URL/api/Auth/login" -H "Content-Type: application/json" -d '{"email": "john.admin@techinnovations.com", "password": "Admin@123"}' | jq -r '.accessToken')

echo "=== Testing Remaining APIs ==="
echo ""

# Task APIs that were not tested
echo "1. Testing PUT /api/Task/{id} - Update Task"
curl -X PUT "$BASE_URL/api/Task/11111111-1111-1111-1111-200000000001" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "11111111-1111-1111-1111-200000000001",
    "title": "Updated Design Homepage Mockup",
    "description": "Updated description for the homepage mockup task",
    "priority": "Critical",
    "progress": 50
  }' | jq

echo -e "\n2. Testing POST /api/Task/{id}/duplicate - Duplicate Task"
curl -X POST "$BASE_URL/api/Task/11111111-1111-1111-1111-200000000001/duplicate" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" \
  -H "Content-Type: application/json" | jq

echo -e "\n3. Testing POST /api/Task/{id}/comment - Add Task Comment"
curl -X POST "$BASE_URL/api/Task/11111111-1111-1111-1111-200000000001/comment" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "comment": "This is a test comment on the task",
    "isInternal": false
  }' | jq

echo -e "\n4. Testing GET /api/Task/test"
curl -X GET "$BASE_URL/api/Task/test" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" | jq

# Project APIs that were not tested
echo -e "\n5. Testing PUT /api/Project/{id} - Update Project"
curl -X PUT "$BASE_URL/api/Project/11111111-1111-1111-1111-100000000001" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "11111111-1111-1111-1111-100000000001",
    "name": "Updated Website Redesign Project",
    "description": "Updated description for the website redesign project",
    "status": "Active",
    "budget": 75000
  }' | jq

echo -e "\n6. Testing POST /api/Project/{id}/team-member - Add Team Member"
curl -X POST "$BASE_URL/api/Project/11111111-1111-1111-1111-100000000001/team-member" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "dddddddd-dddd-dddd-dddd-dddddddddddd"
  }' | jq

echo -e "\n7. Testing DELETE /api/Project/{id}/team-member/{userId} - Remove Team Member"
curl -X DELETE "$BASE_URL/api/Project/11111111-1111-1111-1111-100000000001/team-member/dddddddd-dddd-dddd-dddd-dddddddddddd" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" | jq

# User APIs that were not tested
echo -e "\n8. Testing GET /api/User/{id} - Get User by ID"
curl -X GET "$BASE_URL/api/User/bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" | jq

echo -e "\n9. Testing PUT /api/User/profile - Update Current User Profile"
curl -X PUT "$BASE_URL/api/User/profile" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Administrator",
    "phoneNumber": "+1234567890",
    "department": "Management",
    "jobTitle": "Company Administrator"
  }' | jq

echo -e "\n10. Testing GET /api/User/{id}/tasks - Get User Tasks"
curl -X GET "$BASE_URL/api/User/dddddddd-dddd-dddd-dddd-dddddddddddd/tasks" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" | jq

echo -e "\n11. Testing PUT /api/User/{id}/role - Update User Role"
curl -X PUT "$BASE_URL/api/User/dddddddd-dddd-dddd-dddd-dddddddddddd/role" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "role": "Manager"
  }' | jq

echo -e "\n12. Testing PUT /api/User/{id}/deactivate - Deactivate User"
curl -X PUT "$BASE_URL/api/User/dddddddd-dddd-dddd-dddd-dddddddddddd/deactivate" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" | jq

echo -e "\n13. Testing PUT /api/User/{id}/activate - Activate User"
curl -X PUT "$BASE_URL/api/User/dddddddd-dddd-dddd-dddd-dddddddddddd/activate" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" | jq

echo -e "\n14. Testing GET /api/User/{id}/permissions - Get User Permissions"
curl -X GET "$BASE_URL/api/User/dddddddd-dddd-dddd-dddd-dddddddddddd/permissions" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" | jq

echo -e "\n15. Testing POST /api/User - Create New User"
curl -X POST "$BASE_URL/api/User" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newuser@techinnovations.com",
    "password": "NewUser@123",
    "firstName": "New",
    "lastName": "User",
    "role": "User",
    "department": "Development"
  }' | jq

echo -e "\n16. Testing DELETE /api/Task/{id} - Delete Task (if exists)"
TASK_ID=$(curl -s -X GET "$BASE_URL/api/Task" -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" | jq -r '.items[0].id // empty')
if [ ! -z "$TASK_ID" ]; then
  curl -X DELETE "$BASE_URL/api/Task/$TASK_ID" \
    -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" | jq
else
  echo "No tasks available to delete"
fi

echo -e "\n17. Testing DELETE /api/Project/{id} - Delete Project"
# First create a test project to delete
TEST_PROJECT=$(curl -s -X POST "$BASE_URL/api/Project" \
  -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Project for Deletion",
    "description": "This project will be deleted",
    "projectCode": "TEST-DEL-001",
    "managerId": "cccccccc-cccc-cccc-cccc-cccccccccccc",
    "status": "Planning"
  }')
  
PROJECT_ID=$(echo $TEST_PROJECT | jq -r '.id // empty')
if [ ! -z "$PROJECT_ID" ]; then
  curl -X DELETE "$BASE_URL/api/Project/$PROJECT_ID" \
    -H "Authorization: Bearer $COMPANY_ADMIN_TOKEN" | jq
else
  echo "Failed to create project for deletion test"
fi

echo -e "\n=== All tests completed ==="
