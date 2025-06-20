#!/bin/bash

# Quick Auth API Test
API_URL="http://localhost:5175/api/Auth"

echo "===== QUICK AUTH API TEST ====="
echo ""

# 1. Login Test
echo "1. Testing Login..."
curl -X POST "$API_URL/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "superadmin@taskmanagement.com",
    "password": "Admin@123"
  }' 2>/dev/null | jq . 2>/dev/null || echo "Login request failed"

echo -e "\n2. Testing Forgot Password..."
curl -X POST "$API_URL/forgot-password" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "superadmin@taskmanagement.com"
  }' 2>/dev/null | jq . 2>/dev/null || echo "Forgot password request failed"

echo -e "\n3. Testing Verify Email..."
curl -X GET "$API_URL/verify-email/test-token" 2>/dev/null | jq . 2>/dev/null || echo "Verify email request failed"

echo -e "\n===== TEST COMPLETE ====="