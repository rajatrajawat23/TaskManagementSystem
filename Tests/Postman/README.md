# Task Management System - API Test Suite

## Overview

This comprehensive Postman collection provides complete API testing coverage for the Task Management System. It includes over 80 test cases covering all endpoints, authentication flows, security testing, performance validation, and error handling.

## Collection Features

### 📚 **Complete API Coverage**
- **Authentication Tests**: Login, registration, token refresh, password management
- **Task Management**: Full CRUD operations, assignments, status updates, comments
- **Project Management**: Project lifecycle, team management, statistics
- **Client Management**: Client CRUD operations, project/task relationships
- **Company Management**: Multi-tenant operations, statistics, subscription management
- **User Management**: Profile management, role assignments, permissions
- **Dashboard & Analytics**: Performance metrics, system overview, activity tracking
- **Notification Management**: Real-time notifications, read status management
- **Diagnostics**: System health checks, database validation

### 🛡️ **Security & Validation Testing**
- SQL injection prevention
- XSS attack prevention
- Authentication bypass attempts
- Invalid data format handling
- Large payload stress testing
- UUID format validation

### ⚡ **Performance Testing**
- Response time validation (< 1000ms for most endpoints)
- Load testing with pagination
- Concurrent request simulation
- Search and filtering performance
- Database query optimization validation

### 🧪 **Data Validation**
- Required field validation
- Email format validation
- Date format validation
- Enum value validation
- Cross-field validation

## Quick Start Guide

### 1. **Import Collection**
```bash
# Import the collection into Postman
File > Import > TaskManagement-Complete-Collection.postman_collection.json
```

### 2. **Set Environment Variables**
The collection uses the following variables that auto-populate during testing:

| Variable | Description | Auto-populated |
|----------|-------------|----------------|
| `baseUrl` | API base URL | ✅ Set to `http://localhost:5175/api` |
| `accessToken` | JWT access token | ✅ From login response |
| `refreshToken` | JWT refresh token | ✅ From login response |
| `companyId` | Current company ID | ✅ From login/company response |
| `userId` | Current user ID | ✅ From login response |
| `taskId` | Created task ID | ✅ From task creation |
| `projectId` | Created project ID | ✅ From project creation |
| `clientId` | Created client ID | ✅ From client creation |
| `notificationId` | Notification ID | ✅ From notification tests |

### 3. **Run Test Suite**

#### **Option A: Complete Test Run**
```bash
# Run entire collection (recommended for CI/CD)
newman run TaskManagement-Complete-Collection.postman_collection.json \
  --iteration-count 1 \
  --delay-request 100 \
  --timeout-request 10000
```

#### **Option B: Folder-by-Folder Testing**
1. Start with **🔐 Authentication Tests** to get valid tokens
2. Run any combination of test folders
3. End with **🧹 Cleanup Operations** to logout

#### **Option C: Individual Endpoint Testing**
- Select specific requests for targeted testing
- Ensure authentication is completed first

## Test Execution Flow

### **Recommended Execution Order:**

1. **🔐 Authentication Tests** (Required First)
   - Valid Login - SuperAdmin
   - Register Test User
   - Token refresh testing

2. **🏢 Company Management**
   - Get all companies
   - Create/update company
   - Company statistics

3. **✅ Task Management** 
   - Create tasks
   - CRUD operations
   - Status updates
   - Task assignment
   - Comments and attachments

4. **📁 Project Management**
   - Project lifecycle
   - Team member management
   - Project statistics

5. **👥 Client Management**
   - Client CRUD operations
   - Client-project relationships

6. **👤 User Management**
   - User profiles
   - Role management
   - User tasks

7. **📊 Dashboard & Analytics**
   - Dashboard data
   - Performance metrics
   - Recent activities

8. **🔔 Notification Management**
   - Notification testing
   - Read status management

9. **🔧 Diagnostics & Testing**
   - System health checks
   - Database validation

10. **🛡️ Security & Error Handling Tests**
    - Security vulnerability testing
    - Error scenario validation

11. **⚡ Performance Tests**
    - Load testing
    - Response time validation

12. **🧪 Data Validation Tests**
    - Input validation testing
    - Edge case handling

13. **🔄 Integration Flow Tests**
    - End-to-end workflows
    - Cross-module integration

14. **🧹 Cleanup Operations** (Required Last)
    - Logout and token cleanup

## Test Categories Explained

### **Authentication Tests**
- **Purpose**: Validate JWT authentication, token management, and session handling
- **Key Tests**: Login validation, token refresh, password changes
- **Prerequisites**: Valid SuperAdmin credentials

### **CRUD Operations Tests**
- **Purpose**: Validate all Create, Read, Update, Delete operations
- **Coverage**: Tasks, Projects, Clients, Users, Companies
- **Validation**: Data integrity, relationships, constraints

### **Security Tests**
- **Purpose**: Ensure API security against common attacks
- **Coverage**: SQL injection, XSS, unauthorized access, input validation
- **Expected Results**: Proper error codes (400, 401, 403), no data leakage

### **Performance Tests**
- **Purpose**: Validate API performance under various conditions
- **Metrics**: Response time < 1000ms, proper pagination, concurrent request handling
- **Load Testing**: 100+ record retrieval, search operations

### **Integration Tests**
- **Purpose**: Validate cross-module functionality and workflows
- **Scenarios**: Task assignment workflows, project-client relationships
- **End-to-End**: Complete user journeys from creation to completion

## Expected Test Results

### **Success Criteria**
- ✅ **Authentication**: All login flows work, tokens are properly managed
- ✅ **CRUD Operations**: All entities can be created, read, updated, deleted
- ✅ **Security**: All malicious inputs are properly rejected
- ✅ **Performance**: Response times are under acceptable thresholds
- ✅ **Validation**: Invalid data is properly rejected with appropriate error messages
- ✅ **Integration**: Cross-module workflows function correctly

### **Performance Benchmarks**
- **Login/Authentication**: < 500ms
- **Simple GET requests**: < 300ms
- **Complex queries with joins**: < 500ms
- **CRUD operations**: < 400ms
- **Dashboard/analytics**: < 1000ms
- **File uploads**: < 2000ms

## Troubleshooting

### **Common Issues**

#### **Authentication Failures**
```javascript
// Check if tokens are properly set
console.log('Access Token:', pm.collectionVariables.get('accessToken'));

// Manually set tokens if needed
pm.collectionVariables.set('accessToken', 'your-jwt-token');
```

#### **Test Failures Due to Dependencies**
- Ensure tests run in the correct order
- Authentication must be completed first
- Some tests depend on data created by previous tests

#### **Environment Issues**
- Verify API server is running on `http://localhost:5175`
- Check database connectivity
- Ensure all required seeded data exists

#### **Performance Test Failures**
- Server performance may vary based on hardware
- Adjust performance thresholds in test scripts if needed
- Consider network latency in response time calculations

### **Custom Test Modifications**

#### **Updating Base URL**
```javascript
// Change the base URL for different environments
pm.collectionVariables.set('baseUrl', 'https://your-api-domain.com/api');
```

#### **Adding Custom Validation**
```javascript
// Add custom test validation
pm.test('Custom validation', function () {
    const jsonData = pm.response.json();
    pm.expect(jsonData.customField).to.equal('expectedValue');
});
```

#### **Environment-Specific Testing**
```javascript
// Conditional tests based on environment
if (pm.environment.get('environment') === 'production') {
    pm.test('Production-specific test', function () {
        // Production-only validation
    });
}
```

## Advanced Usage

### **CI/CD Integration**
```yaml
# GitHub Actions example
- name: Run API Tests
  run: |
    newman run Tests/Postman/TaskManagement-Complete-Collection.postman_collection.json \
      --reporters cli,json \
      --reporter-json-export test-results.json
```

### **Automated Testing Scripts**
```bash
#!/bin/bash
# automated-test-runner.sh

echo "Starting Task Management API Test Suite..."

# Run the complete test suite
newman run TaskManagement-Complete-Collection.postman_collection.json \
  --iteration-count 1 \
  --delay-request 100 \
  --timeout-request 10000 \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export api-test-report.html

echo "Test execution completed. Check api-test-report.html for detailed results."
```

### **Data-Driven Testing**
```csv
# test-data.csv
email,password,expectedRole
admin@test.com,Admin@123,SuperAdmin
manager@test.com,Manager@123,Manager
user@test.com,User@123,User
```

## Contributing

### **Adding New Tests**
1. Follow the existing naming convention: `🔧 Category Name`
2. Include comprehensive test validation
3. Add proper error handling
4. Update this README with new test descriptions

### **Test Best Practices**
- Always include status code validation
- Validate response structure and data types
- Include negative testing scenarios
- Add performance benchmarks where applicable
- Use descriptive test names and error messages

## Support

For issues related to:
- **Collection Structure**: Check folder organization and test dependencies
- **API Endpoints**: Verify against the actual API implementation
- **Performance Issues**: Review server configuration and database optimization
- **Authentication Problems**: Validate user credentials and token configuration

## Collection Statistics

- **Total Requests**: 80+
- **Test Categories**: 13
- **Security Tests**: 6
- **Performance Tests**: 4
- **Integration Tests**: 3
- **Validation Tests**: 4
- **Coverage**: All API endpoints and major workflows

---

**Last Updated**: June 2025  
**Version**: 3.0  
**Compatibility**: Task Management System API v1
