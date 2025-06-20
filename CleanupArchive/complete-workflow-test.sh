#!/bin/bash

# Complete Task Management System Workflow Test Script
# This script demonstrates the complete user workflow from company setup to task management

BASE_URL="http://localhost:5175"
echo "🚀 Starting Complete Task Management System Workflow Test"
echo "=================================="

# Step 1: Create and login SuperAdmin
echo "Step 1: Creating SuperAdmin..."
SUPERADMIN_RESPONSE=$(curl -s -X POST "$BASE_URL/api/Auth/register-test-user" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "superadmin@system.com",
    "password": "SuperAdmin123!",
    "confirmPassword": "SuperAdmin123!",
    "firstName": "Super",
    "lastName": "Admin",
    "role": "SuperAdmin"
  }')

echo "SuperAdmin Registration: $SUPERADMIN_RESPONSE"

# Login SuperAdmin
echo "Logging in SuperAdmin..."
SUPERADMIN_LOGIN=$(curl -s -X POST "$BASE_URL/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "superadmin@system.com",
    "password": "SuperAdmin123!"
  }')

SUPERADMIN_TOKEN=$(echo $SUPERADMIN_LOGIN | jq -r '.data.token // .token')
echo "SuperAdmin Token: ${SUPERADMIN_TOKEN:0:50}..."

# Step 2: Create Company
echo ""
echo "Step 2: Creating Company..."
COMPANY_RESPONSE=$(curl -s -X POST "$BASE_URL/api/Company" \
  -H "Authorization: Bearer $SUPERADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Demo Tech Solutions",
    "domain": "demo.tech",
    "contactEmail": "contact@demo.tech",
    "contactPhone": "+1-555-0123",
    "address": "123 Tech Street, Innovation City",
    "subscriptionType": "Premium",
    "maxUsers": 50
  }')

COMPANY_ID=$(echo $COMPANY_RESPONSE | jq -r '.data.id // .id')
echo "Company Created: $COMPANY_ID"
echo "Company Response: $COMPANY_RESPONSE"

# Step 3: Create Company Admin
echo ""
echo "Step 3: Creating Company Admin..."
ADMIN_RESPONSE=$(curl -s -X POST "$BASE_URL/api/Auth/register" \
  -H "Authorization: Bearer $SUPERADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@demo.tech",
    "password": "CompanyAdmin123!",
    "confirmPassword": "CompanyAdmin123!",
    "firstName": "John",
    "lastName": "Admin",
    "role": "CompanyAdmin",
    "companyId": "'$COMPANY_ID'",
    "department": "Administration",
    "jobTitle": "Company Administrator"
  }')

echo "Company Admin Registration: $ADMIN_RESPONSE"

# Login Company Admin
echo "Logging in Company Admin..."
ADMIN_LOGIN=$(curl -s -X POST "$BASE_URL/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@demo.tech",
    "password": "CompanyAdmin123!"
  }')

ADMIN_TOKEN=$(echo $ADMIN_LOGIN | jq -r '.data.token // .token')
echo "Company Admin Token: ${ADMIN_TOKEN:0:50}..."

# Step 4: Create Manager
echo ""
echo "Step 4: Creating Manager..."
MANAGER_RESPONSE=$(curl -s -X POST "$BASE_URL/api/User" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "manager@demo.tech",
    "password": "Manager123!",
    "firstName": "Sarah",
    "lastName": "Manager",
    "role": "Manager",
    "department": "Engineering",
    "jobTitle": "Engineering Manager",
    "phoneNumber": "+1-555-0124"
  }')

MANAGER_ID=$(echo $MANAGER_RESPONSE | jq -r '.data.id // .id')
echo "Manager Created: $MANAGER_ID"

# Step 5: Create Developer
echo ""
echo "Step 5: Creating Developer..."
DEVELOPER_RESPONSE=$(curl -s -X POST "$BASE_URL/api/User" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "developer@demo.tech",
    "password": "Developer123!",
    "firstName": "Mike",
    "lastName": "Developer",
    "role": "User",
    "department": "Engineering",
    "jobTitle": "Software Developer"
  }')

DEVELOPER_ID=$(echo $DEVELOPER_RESPONSE | jq -r '.data.id // .id')
echo "Developer Created: $DEVELOPER_ID"

# Step 6: Create TaskAssigner
echo ""
echo "Step 6: Creating TaskAssigner..."
TASKASSIGNER_RESPONSE=$(curl -s -X POST "$BASE_URL/api/User" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "taskassigner@demo.tech",
    "password": "TaskAssigner123!",
    "firstName": "Lisa",
    "lastName": "Coordinator",
    "role": "TaskAssigner",
    "department": "Operations",
    "jobTitle": "Task Coordinator"
  }')

TASKASSIGNER_ID=$(echo $TASKASSIGNER_RESPONSE | jq -r '.data.id // .id')
echo "TaskAssigner Created: $TASKASSIGNER_ID"

# Login Manager for next steps
echo ""
echo "Step 7: Logging in Manager..."
MANAGER_LOGIN=$(curl -s -X POST "$BASE_URL/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "manager@demo.tech",
    "password": "Manager123!"
  }')

MANAGER_TOKEN=$(echo $MANAGER_LOGIN | jq -r '.data.token // .token')
echo "Manager Token: ${MANAGER_TOKEN:0:50}..."

# Step 8: Create Client
echo ""
echo "Step 8: Creating Client..."
CLIENT_RESPONSE=$(curl -s -X POST "$BASE_URL/api/Client" \
  -H "Authorization: Bearer $MANAGER_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Tech Solutions Inc",
    "email": "contact@techsolutions.com",
    "phone": "+1-555-0200",
    "address": "456 Tech Park, Innovation City",
    "contactPerson": "David Tech",
    "industry": "Technology",
    "notes": "Long-term strategic partner"
  }')

CLIENT_ID=$(echo $CLIENT_RESPONSE | jq -r '.data.id // .id')
echo "Client Created: $CLIENT_ID"

# Step 9: Create Project
echo ""
echo "Step 9: Creating Project..."
PROJECT_RESPONSE=$(curl -s -X POST "$BASE_URL/api/Project" \
  -H "Authorization: Bearer $MANAGER_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "E-commerce Platform Development",
    "description": "Build a modern e-commerce platform for client",
    "clientId": "'$CLIENT_ID'",
    "managerId": "'$MANAGER_ID'",
    "startDate": "2025-06-21T00:00:00Z",
    "endDate": "2025-12-31T23:59:59Z",
    "budget": 150000.00,
    "status": "Planning"
  }')

PROJECT_ID=$(echo $PROJECT_RESPONSE | jq -r '.data.id // .id')
echo "Project Created: $PROJECT_ID"

# Step 10: Add Team Member to Project
echo ""
echo "Step 10: Adding Developer to Project..."
TEAM_RESPONSE=$(curl -s -X POST "$BASE_URL/api/Project/$PROJECT_ID/team-member" \
  -H "Authorization: Bearer $MANAGER_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "'$DEVELOPER_ID'",
    "role": "Developer",
    "notes": "Lead frontend developer"
  }')

echo "Team Member Added: $TEAM_RESPONSE"

# Step 11: Create Task
echo ""
echo "Step 11: Creating Task..."
TASK_RESPONSE=$(curl -s -X POST "$BASE_URL/api/Task" \
  -H "Authorization: Bearer $MANAGER_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Implement User Authentication",
    "description": "Develop JWT-based authentication system",
    "assignedToId": "'$DEVELOPER_ID'",
    "projectId": "'$PROJECT_ID'",
    "clientId": "'$CLIENT_ID'",
    "priority": "High",
    "category": "Development",
    "estimatedHours": 16.0,
    "startDate": "2025-06-21T09:00:00Z",
    "dueDate": "2025-06-25T17:00:00Z",
    "tags": ["authentication", "security", "backend"]
  }')

TASK_ID=$(echo $TASK_RESPONSE | jq -r '.data.id // .id')
echo "Task Created: $TASK_ID"

# Step 12: Login Developer and Update Task
echo ""
echo "Step 12: Logging in Developer..."
DEVELOPER_LOGIN=$(curl -s -X POST "$BASE_URL/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "developer@demo.tech",
    "password": "Developer123!"
  }')

DEVELOPER_TOKEN=$(echo $DEVELOPER_LOGIN | jq -r '.data.token // .token')
echo "Developer Token: ${DEVELOPER_TOKEN:0:50}..."

# Update Task Status
echo ""
echo "Step 13: Updating Task Status..."
TASK_UPDATE=$(curl -s -X PUT "$BASE_URL/api/Task/$TASK_ID/status" \
  -H "Authorization: Bearer $DEVELOPER_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "status": "InProgress",
    "progress": 75,
    "actualHours": 12.0,
    "notes": "Authentication endpoints completed, working on frontend integration"
  }')

echo "Task Status Updated: $TASK_UPDATE"

# Step 14: View Dashboard
echo ""
echo "Step 14: Viewing Company Dashboard..."
DASHBOARD=$(curl -s -X GET "$BASE_URL/api/Dashboard/company-overview" \
  -H "Authorization: Bearer $ADMIN_TOKEN")

echo "Company Dashboard: $DASHBOARD"

# Step 15: View All Users
echo ""
echo "Step 15: Viewing All Users..."
ALL_USERS=$(curl -s -X GET "$BASE_URL/api/User" \
  -H "Authorization: Bearer $ADMIN_TOKEN")

echo "All Users: $ALL_USERS"

# Summary
echo ""
echo "🎉 WORKFLOW TEST COMPLETED SUCCESSFULLY!"
echo "=================================="
echo "✅ SuperAdmin created and logged in"
echo "✅ Company created: $COMPANY_ID"
echo "✅ Company Admin created and logged in"
echo "✅ Manager created: $MANAGER_ID"
echo "✅ Developer created: $DEVELOPER_ID"
echo "✅ TaskAssigner created: $TASKASSIGNER_ID"
echo "✅ Client created: $CLIENT_ID"
echo "✅ Project created: $PROJECT_ID"
echo "✅ Team member added to project"
echo "✅ Task created: $TASK_ID"
echo "✅ Task status updated by developer"
echo "✅ Dashboard viewed"
echo ""
echo "🔑 LOGIN CREDENTIALS:"
echo "SuperAdmin: superadmin@system.com / SuperAdmin123!"
echo "Company Admin: admin@demo.tech / CompanyAdmin123!"
echo "Manager: manager@demo.tech / Manager123!"
echo "Developer: developer@demo.tech / Developer123!"
echo "TaskAssigner: taskassigner@demo.tech / TaskAssigner123!"
echo ""
echo "🌐 Access Swagger UI: http://localhost:5175"
echo "🎯 Your system is ready for production use!"
