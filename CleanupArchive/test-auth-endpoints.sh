#!/bin/bash

# API Base URL - using the correct port 5175
API_URL="http://localhost:5175/api/Auth"

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=== Testing Auth API Endpoints ===${NC}\n"

# Test data - Using correct password from seed data (Admin@123)
TEST_EMAIL="test.user@techinnovations.com"
TEST_PASSWORD="TestPassword123!"
ADMIN_EMAIL="john.admin@techinnovations.com"
ADMIN_PASSWORD="Admin@123"
SEED_PASSWORD="Admin@123"  # Password used in seed data

# Variables to store tokens
ACCESS_TOKEN=""
REFRESH_TOKEN=""

# 1. Test Registration (This requires authentication as CompanyAdmin or SuperAdmin)
echo -e "${GREEN}1. Testing POST /api/Auth/register${NC}"
echo "Note: This endpoint requires authentication. Testing without auth first..."
REGISTER_RESPONSE=$(curl -s -X POST "$API_URL/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "'$TEST_EMAIL'",
    "password": "'$TEST_PASSWORD'",
    "confirmPassword": "'$TEST_PASSWORD'",
    "firstName": "Test",
    "lastName": "User",
    "phoneNumber": "123-456-7890",
    "companyId": "11111111-1111-1111-1111-111111111111",
    "role": "User",
    "department": "Engineering",
    "jobTitle": "Software Developer"
  }')
echo "Response: $REGISTER_RESPONSE"
echo -e "\n"

# 2. Test Login with existing user
echo -e "${GREEN}2. Testing POST /api/Auth/login${NC}"
echo "Testing with SuperAdmin credentials..."
LOGIN_RESPONSE=$(curl -s -X POST "$API_URL/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "superadmin@taskmanagement.com",
    "password": "Admin@123"
  }')
echo "Response: $LOGIN_RESPONSE"

# Try extracting tokens if login successful
if [[ $LOGIN_RESPONSE == *"accessToken"* ]]; then
  ACCESS_TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)
  REFRESH_TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"refreshToken":"[^"]*' | cut -d'"' -f4)
  echo "Access Token extracted: ${ACCESS_TOKEN:0:50}..."
fi
echo -e "\n"

# 3. Test Refresh Token
echo -e "${GREEN}3. Testing POST /api/Auth/refresh-token${NC}"
if [ -n "$ACCESS_TOKEN" ] && [ -n "$REFRESH_TOKEN" ]; then
  REFRESH_RESPONSE=$(curl -s -X POST "$API_URL/refresh-token" \
    -H "Content-Type: application/json" \
    -d '{
      "accessToken": "'$ACCESS_TOKEN'",
      "refreshToken": "'$REFRESH_TOKEN'"
    }')
  echo "Response: $REFRESH_RESPONSE"
else
  echo "Skipping - No tokens available"
fi
echo -e "\n"

# 4. Test Change Password (Requires authentication)
echo -e "${GREEN}4. Testing POST /api/Auth/change-password${NC}"
if [ -n "$ACCESS_TOKEN" ]; then
  CHANGE_PWD_RESPONSE=$(curl -s -X POST "$API_URL/change-password" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $ACCESS_TOKEN" \
    -d '{
      "currentPassword": "Admin@123",
      "newPassword": "NewPassword123!",
      "confirmPassword": "NewPassword123!"
    }')
  echo "Response: $CHANGE_PWD_RESPONSE"
else
  echo "Skipping - No access token available"
fi
echo -e "\n"

# 5. Test Forgot Password
echo -e "${GREEN}5. Testing POST /api/Auth/forgot-password${NC}"
FORGOT_PWD_RESPONSE=$(curl -s -X POST "$API_URL/forgot-password" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "superadmin@taskmanagement.com"
  }')
echo "Response: $FORGOT_PWD_RESPONSE"
echo -e "\n"

# 6. Test Reset Password
echo -e "${GREEN}6. Testing POST /api/Auth/reset-password${NC}"
echo "Using a sample token (would normally come from email)..."
RESET_PWD_RESPONSE=$(curl -s -X POST "$API_URL/reset-password" \
  -H "Content-Type: application/json" \
  -d '{
    "token": "sample-reset-token-123",
    "newPassword": "ResetPassword123!",
    "confirmPassword": "ResetPassword123!"
  }')
echo "Response: $RESET_PWD_RESPONSE"
echo -e "\n"

# 7. Test Verify Email
echo -e "${GREEN}7. Testing GET /api/Auth/verify-email/{token}${NC}"
VERIFY_EMAIL_RESPONSE=$(curl -s -X GET "$API_URL/verify-email/sample-verification-token-123")
echo "Response: $VERIFY_EMAIL_RESPONSE"
echo -e "\n"

# 8. Test Logout (Requires authentication)
echo -e "${GREEN}8. Testing POST /api/Auth/logout${NC}"
if [ -n "$ACCESS_TOKEN" ]; then
  LOGOUT_RESPONSE=$(curl -s -X POST "$API_URL/logout" \
    -H "Authorization: Bearer $ACCESS_TOKEN")
  echo "Response: $LOGOUT_RESPONSE"
else
  echo "Skipping - No access token available"
fi
echo -e "\n"

echo -e "${BLUE}=== Auth API Testing Complete ===${NC}"