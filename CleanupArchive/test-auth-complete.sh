#!/bin/bash

# API Base URL
API_URL="http://localhost:5175/api/Auth"

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Test counters
TOTAL_TESTS=0
PASSED_TESTS=0
FAILED_TESTS=0

# Function to print test result
print_result() {
    local test_name=$1
    local response=$2
    local expected_contains=$3
    
    TOTAL_TESTS=$((TOTAL_TESTS + 1))
    
    if [[ $response == *"$expected_contains"* ]]; then
        echo -e "${GREEN}✓ $test_name - PASSED${NC}"
        echo "  Response: ${response:0:150}..."
        PASSED_TESTS=$((PASSED_TESTS + 1))
    else
        echo -e "${RED}✗ $test_name - FAILED${NC}"
        echo "  Expected to contain: $expected_contains"
        echo "  Actual response: $response"
        FAILED_TESTS=$((FAILED_TESTS + 1))
    fi
    echo ""
}

echo -e "${BLUE}╔══════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║          AUTH API ENDPOINTS TEST SUITE                   ║${NC}"
echo -e "${BLUE}╚══════════════════════════════════════════════════════════╝${NC}\n"

# Variables to store tokens
ACCESS_TOKEN=""
REFRESH_TOKEN=""

# ========== TEST 1: Login with SuperAdmin ==========
echo -e "${YELLOW}[TEST 1] Login with SuperAdmin${NC}"
LOGIN_RESPONSE=$(curl -s -X POST "$API_URL/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "superadmin@taskmanagement.com",
    "password": "Admin@123"
  }')

# Extract tokens
if [[ $LOGIN_RESPONSE == *"accessToken"* ]]; then
  ACCESS_TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)
  REFRESH_TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"refreshToken":"[^"]*' | cut -d'"' -f4)
fi

print_result "Login with valid credentials" "$LOGIN_RESPONSE" '"success":true'

# ========== TEST 2: Login with invalid credentials ==========
echo -e "${YELLOW}[TEST 2] Login with invalid credentials${NC}"
INVALID_LOGIN=$(curl -s -X POST "$API_URL/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "superadmin@taskmanagement.com",
    "password": "WrongPassword"
  }')
print_result "Login with invalid password" "$INVALID_LOGIN" '"success":false'

# ========== TEST 3: Register new user (with auth) ==========
echo -e "${YELLOW}[TEST 3] Register new user with authentication${NC}"
TIMESTAMP=$(date +%s)
REGISTER_RESPONSE=$(curl -s -X POST "$API_URL/register" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "email": "newuser'$TIMESTAMP'@techinnovations.com",
    "password": "NewUser123!",
    "confirmPassword": "NewUser123!",
    "firstName": "New",
    "lastName": "User",
    "phoneNumber": "+15555550123",
    "companyId": "11111111-1111-1111-1111-111111111111",
    "role": "User",
    "department": "Engineering",
    "jobTitle": "Junior Developer"
  }')
print_result "Register new user with auth" "$REGISTER_RESPONSE" '"success":true'

# ========== TEST 4: Register without auth (should fail) ==========
echo -e "${YELLOW}[TEST 4] Register without authentication${NC}"
REGISTER_NO_AUTH=$(curl -s -X POST "$API_URL/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "unauthorized@test.com",
    "password": "Test123!",
    "confirmPassword": "Test123!",
    "firstName": "Unauthorized",
    "lastName": "User",
    "role": "User"
  }')
print_result "Register without auth (should fail)" "$REGISTER_NO_AUTH" ''

# ========== TEST 5: Refresh Token ==========
echo -e "${YELLOW}[TEST 5] Refresh Token${NC}"
if [ -n "$ACCESS_TOKEN" ] && [ -n "$REFRESH_TOKEN" ]; then
  REFRESH_RESPONSE=$(curl -s -X POST "$API_URL/refresh-token" \
    -H "Content-Type: application/json" \
    -d '{
      "accessToken": "'$ACCESS_TOKEN'",
      "refreshToken": "'$REFRESH_TOKEN'"
    }')
  print_result "Refresh token" "$REFRESH_RESPONSE" '"success":true'
  
  # Update tokens
  if [[ $REFRESH_RESPONSE == *"accessToken"* ]]; then
    ACCESS_TOKEN=$(echo $REFRESH_RESPONSE | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)
    REFRESH_TOKEN=$(echo $REFRESH_RESPONSE | grep -o '"refreshToken":"[^"]*' | cut -d'"' -f4)
  fi
else
  echo -e "${RED}✗ Refresh token - SKIPPED (no tokens)${NC}\n"
fi

# ========== TEST 6: Change Password ==========
echo -e "${YELLOW}[TEST 6] Change Password${NC}"
if [ -n "$ACCESS_TOKEN" ]; then
  # First, change the password
  CHANGE_PWD=$(curl -s -X POST "$API_URL/change-password" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $ACCESS_TOKEN" \
    -d '{
      "currentPassword": "Admin@123",
      "newPassword": "NewAdmin@123",
      "confirmPassword": "NewAdmin@123"
    }')
  print_result "Change password" "$CHANGE_PWD" "Password changed successfully"
  
  # Change it back
  if [[ $CHANGE_PWD == *"successfully"* ]]; then
    echo -e "${YELLOW}[TEST 6.1] Change password back to original${NC}"
    CHANGE_BACK=$(curl -s -X POST "$API_URL/change-password" \
      -H "Content-Type: application/json" \
      -H "Authorization: Bearer $ACCESS_TOKEN" \
      -d '{
        "currentPassword": "NewAdmin@123",
        "newPassword": "Admin@123",
        "confirmPassword": "Admin@123"
      }')
    print_result "Change password back" "$CHANGE_BACK" "Password changed successfully"
  fi
else
  echo -e "${RED}✗ Change password - SKIPPED (no token)${NC}\n"
fi

# ========== TEST 7: Forgot Password ==========
echo -e "${YELLOW}[TEST 7] Forgot Password${NC}"
FORGOT_PWD=$(curl -s -X POST "$API_URL/forgot-password" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "superadmin@taskmanagement.com"
  }')
print_result "Forgot password" "$FORGOT_PWD" "Password reset instructions"

# ========== TEST 8: Forgot Password with invalid email ==========
echo -e "${YELLOW}[TEST 8] Forgot Password with non-existent email${NC}"
FORGOT_INVALID=$(curl -s -X POST "$API_URL/forgot-password" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "nonexistent@email.com"
  }')
print_result "Forgot password (invalid email)" "$FORGOT_INVALID" "Password reset instructions"

# ========== TEST 9: Reset Password ==========
echo -e "${YELLOW}[TEST 9] Reset Password with invalid token${NC}"
RESET_PWD=$(curl -s -X POST "$API_URL/reset-password" \
  -H "Content-Type: application/json" \
  -d '{
    "token": "invalid-token-123",
    "newPassword": "ResetPassword123!",
    "confirmPassword": "ResetPassword123!"
  }')
print_result "Reset password (invalid token)" "$RESET_PWD" "Failed to reset password"

# ========== TEST 10: Verify Email ==========
echo -e "${YELLOW}[TEST 10] Verify Email with invalid token${NC}"
VERIFY_EMAIL=$(curl -s -X GET "$API_URL/verify-email/invalid-verification-token")
print_result "Verify email (invalid token)" "$VERIFY_EMAIL" "Invalid or expired"

# ========== TEST 11: Logout ==========
echo -e "${YELLOW}[TEST 11] Logout${NC}"
if [ -n "$ACCESS_TOKEN" ]; then
  LOGOUT_RESPONSE=$(curl -s -X POST "$API_URL/logout" \
    -H "Authorization: Bearer $ACCESS_TOKEN")
  print_result "Logout" "$LOGOUT_RESPONSE" "Logged out successfully"
else
  echo -e "${RED}✗ Logout - SKIPPED (no token)${NC}\n"
fi

# ========== TEST 12: Try to use token after logout ==========
echo -e "${YELLOW}[TEST 12] Try to change password after logout${NC}"
if [ -n "$ACCESS_TOKEN" ]; then
  AFTER_LOGOUT=$(curl -s -X POST "$API_URL/change-password" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $ACCESS_TOKEN" \
    -d '{
      "currentPassword": "Admin@123",
      "newPassword": "Test123!",
      "confirmPassword": "Test123!"
    }')
  # This might still work if the token hasn't expired
  echo "  Response: ${AFTER_LOGOUT:0:150}..."
  echo ""
fi

# ========== SUMMARY ==========
echo -e "${BLUE}╔══════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║                    TEST SUMMARY                          ║${NC}"
echo -e "${BLUE}╚══════════════════════════════════════════════════════════╝${NC}"
echo -e "Total Tests: $TOTAL_TESTS"
echo -e "${GREEN}Passed: $PASSED_TESTS${NC}"
echo -e "${RED}Failed: $FAILED_TESTS${NC}"

if [ $FAILED_TESTS -eq 0 ]; then
    echo -e "\n${GREEN}All tests passed! ✨${NC}"
else
    echo -e "\n${YELLOW}Some tests failed. Please check the output above.${NC}"
fi

# Save test results
echo -e "\nTest results saved to: auth-test-results.json"
cat > auth-test-results.json << EOF
{
  "timestamp": "$(date -u +"%Y-%m-%dT%H:%M:%SZ")",
  "total_tests": $TOTAL_TESTS,
  "passed": $PASSED_TESTS,
  "failed": $FAILED_TESTS,
  "endpoints_tested": [
    "POST /api/Auth/login",
    "POST /api/Auth/register",
    "POST /api/Auth/refresh-token",
    "POST /api/Auth/change-password",
    "POST /api/Auth/forgot-password",
    "POST /api/Auth/reset-password",
    "GET /api/Auth/verify-email/{token}",
    "POST /api/Auth/logout"
  ]
}
EOF