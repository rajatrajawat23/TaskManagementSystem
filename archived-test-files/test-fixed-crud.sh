#!/bin/bash

API_BASE="http://localhost:5175/api"
TOKEN=""

echo "=== Testing Fixed CRUD Operations ==="
echo "Date: $(date)"
echo ""

# 1. Login first
echo "1. Testing Login..."
LOGIN_RESPONSE=$(curl -s -X POST "$API_BASE/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "superadmin@taskmanagement.com",
    "password": "Admin@123"
  }')

TOKEN=$(echo $LOGIN_RESPONSE | jq -r '.accessToken')

if [ "$TOKEN" != "null" ] && [ -n "$TOKEN" ]; then
  echo "✅ Login successful"
else
  echo "❌ Login failed"
  echo "Response: $LOGIN_RESPONSE"
  exit 1
fi

# 2. Test GET operations (should work)
echo -e "\n2. Testing GET operations..."

# Get all companies
echo -e "\n2.1 Getting all companies..."
COMPANIES=$(curl -s -X GET "$API_BASE/company" \
  -H "Authorization: Bearer $TOKEN")
echo "Companies count: $(echo $COMPANIES | jq '.totalCount')"

# Get all tasks
echo -e "\n2.2 Getting all tasks..."
TASKS=$(curl -s -X GET "$API_BASE/task" \
  -H "Authorization: Bearer $TOKEN")
echo "Tasks count: $(echo $TASKS | jq '.totalCount')"

# Get task statistics
echo -e "\n2.3 Getting task statistics..."
STATS=$(curl -s -X GET "$API_BASE/task/statistics" \
  -H "Authorization: Bearer $TOKEN")
echo "Total tasks: $(echo $STATS | jq '.totalTasks')"

# 3. Test CREATE operations with proper data
echo -e "\n3. Testing CREATE operations..."

# Create a task with proper companyId
echo -e "\n3.1 Creating a task..."
CREATE_TASK=$(curl -s -X POST "$API_BASE/task" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Fixed CRUD Test Task",
    "description": "Testing after fixing DTOs",
    "priority": "High",
    "status": "Pending",
    "companyId": "11111111-1111-1111-1111-111111111111",
    "category": "Development",
    "estimatedHours": 8,
    "dueDate": "2025-06-25T00:00:00Z"
  }')

TASK_ID=$(echo $CREATE_TASK | jq -r '.id')
if [ "$TASK_ID" != "null" ] && [ -n "$TASK_ID" ]; then
  echo "✅ Task created successfully. ID: $TASK_ID"
else
  echo "❌ Task creation failed"
  echo "Response: $CREATE_TASK"
fi

# 4. Test UPDATE operations
echo -e "\n4. Testing UPDATE operations..."

if [ "$TASK_ID" != "null" ] && [ -n "$TASK_ID" ]; then
  echo -e "\n4.1 Updating task..."
  UPDATE_TASK=$(curl -s -X PUT "$API_BASE/task/$TASK_ID" \
    -H "Authorization: Bearer $TOKEN" \
    -H "Content-Type: application/json" \
    -d '{
      "id": "'$TASK_ID'",
      "title": "Updated Task Title",
      "description": "Updated description",
      "priority": "Critical",
      "status": "InProgress",
      "progress": 50,
      "companyId": "11111111-1111-1111-1111-111111111111"
    }')
  
  UPDATE_STATUS=$(echo $UPDATE_TASK | jq -r '.id')
  if [ "$UPDATE_STATUS" != "null" ]; then
    echo "✅ Task updated successfully"
  else
    echo "❌ Task update failed"
    echo "Response: $UPDATE_TASK"
  fi
  
  echo -e "\n4.2 Updating task status..."
  STATUS_UPDATE=$(curl -s -X PUT "$API_BASE/task/$TASK_ID/status" \
    -H "Authorization: Bearer $TOKEN" \
    -H "Content-Type: application/json" \
    -d '{
      "status": "Review",
      "comment": "Ready for review"
    }')
  
  STATUS_CHECK=$(echo $STATUS_UPDATE | jq -r '.status')
  if [ "$STATUS_CHECK" == "Review" ]; then
    echo "✅ Task status updated successfully"
  else
    echo "❌ Task status update failed"
    echo "Response: $STATUS_UPDATE"
  fi
fi

# 5. Test Company operations
echo -e "\n5. Testing Company operations..."

echo -e "\n5.1 Creating a company with admin user..."
CREATE_COMPANY=$(curl -s -X POST "$API_BASE/company" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Company Fixed",
    "domain": "testfixed",
    "contactEmail": "contact@testfixed.com",
    "subscriptionType": "Premium",
    "maxUsers": 25,
    "adminEmail": "admin@testfixed.com",
    "adminPassword": "Admin@123",
    "adminFirstName": "Test",
    "adminLastName": "Admin"
  }')

COMPANY_ID=$(echo $CREATE_COMPANY | jq -r '.id')
if [ "$COMPANY_ID" != "null" ] && [ -n "$COMPANY_ID" ]; then
  echo "✅ Company created successfully. ID: $COMPANY_ID"
else
  echo "❌ Company creation failed"
  echo "Response: $CREATE_COMPANY"
fi

# 6. Test User operations
echo -e "\n6. Testing User operations..."

# First, switch to a company context
echo -e "\n6.1 Getting user profile..."
USER_PROFILE=$(curl -s -X GET "$API_BASE/user/profile" \
  -H "Authorization: Bearer $TOKEN")

echo "Current user: $(echo $USER_PROFILE | jq -r '.email')"

# 7. Test DELETE operations
echo -e "\n7. Testing DELETE operations..."

if [ "$TASK_ID" != "null" ] && [ -n "$TASK_ID" ]; then
  echo -e "\n7.1 Deleting task..."
  DELETE_TASK=$(curl -s -X DELETE "$API_BASE/task/$TASK_ID" \
    -H "Authorization: Bearer $TOKEN")
  
  DELETE_STATUS=$(echo $DELETE_TASK | head -c 1)
  if [ -z "$DELETE_STATUS" ]; then
    echo "✅ Task deleted successfully (204 No Content)"
  else
    echo "❌ Task deletion failed"
    echo "Response: $DELETE_TASK"
  fi
fi

# 8. Test Client creation
echo -e "\n8. Testing Client operations..."

echo -e "\n8.1 Creating a client..."
CREATE_CLIENT=$(curl -s -X POST "$API_BASE/client" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Client Fixed",
    "email": "client@testfixed.com",
    "phone": "+1234567890",
    "address": "123 Test Street",
    "companyId": "11111111-1111-1111-1111-111111111111"
  }')

CLIENT_ID=$(echo $CREATE_CLIENT | jq -r '.id')
if [ "$CLIENT_ID" != "null" ] && [ -n "$CLIENT_ID" ]; then
  echo "✅ Client created successfully. ID: $CLIENT_ID"
else
  echo "❌ Client creation failed"
  echo "Response: $CREATE_CLIENT"
fi

# 9. Test Project creation
echo -e "\n9. Testing Project operations..."

echo -e "\n9.1 Creating a project..."
CREATE_PROJECT=$(curl -s -X POST "$API_BASE/project" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Project Fixed",
    "description": "Testing fixed CRUD operations",
    "status": "Active",
    "priority": "High",
    "companyId": "11111111-1111-1111-1111-111111111111",
    "managerId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    "startDate": "2025-06-18T00:00:00Z",
    "endDate": "2025-12-31T00:00:00Z"
  }')

PROJECT_ID=$(echo $CREATE_PROJECT | jq -r '.id')
if [ "$PROJECT_ID" != "null" ] && [ -n "$PROJECT_ID" ]; then
  echo "✅ Project created successfully. ID: $PROJECT_ID"
else
  echo "❌ Project creation failed"
  echo "Response: $CREATE_PROJECT"
fi

echo -e "\n=== Summary ==="
echo "Test completed at: $(date)"
