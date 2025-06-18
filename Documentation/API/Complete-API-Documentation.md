# Task Management System - Backend API Documentation

## Overview

The Task Management System backend is a comprehensive multi-tenant SaaS application built with ASP.NET Core 8, providing enterprise-grade task management capabilities with complete data isolation between companies.

## Architecture

### Technology Stack
- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server 2022 (Docker)
- **ORM**: Entity Framework Core 8.0
- **Authentication**: JWT Bearer Tokens
- **Real-time**: SignalR
- **Documentation**: Swagger/OpenAPI
- **Architecture Pattern**: Clean Architecture
- **Design Patterns**: Repository, Unit of Work, SOLID principles

### Project Structure
```
TaskManagementSystem/
├── Backend/
│   ├── TaskManagement.API/          # Web API Layer
│   ├── TaskManagement.Core/         # Domain Layer
│   └── TaskManagement.Infrastructure/ # Data Access Layer
├── Database/
│   ├── Docker/                      # Docker configuration
│   └── Scripts/                     # SQL scripts
└── Documentation/                   # Project documentation
```

## API Endpoints

### Authentication (`/api/auth`)
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - Register new user
- `POST /api/auth/refresh-token` - Refresh JWT token
- `POST /api/auth/logout` - Logout user
- `POST /api/auth/forgot-password` - Request password reset
- `POST /api/auth/reset-password` - Reset password
- `GET /api/auth/verify-email/{token}` - Verify email address
- `POST /api/auth/change-password` - Change password

### Tasks (`/api/task`)
- `GET /api/task` - Get all tasks (with filtering, sorting, pagination)
- `GET /api/task/{id}` - Get task by ID
- `POST /api/task` - Create new task
- `PUT /api/task/{id}` - Update task
- `DELETE /api/task/{id}` - Delete task (soft delete)
- `POST /api/task/{id}/assign` - Assign task to user
- `PUT /api/task/{id}/status` - Update task status
- `GET /api/task/calendar/{year}/{month}` - Calendar view
- `GET /api/task/user/{userId}` - Get user's tasks
- `GET /api/task/overdue` - Get overdue tasks
- `POST /api/task/{id}/duplicate` - Duplicate task
- `GET /api/task/statistics` - Task statistics
- `POST /api/task/{id}/comment` - Add comment
- `POST /api/task/{id}/attachment` - Add attachment

### Users (`/api/user`)
- `GET /api/user` - Get all users (Manager+)
- `GET /api/user/{id}` - Get user by ID
- `GET /api/user/profile` - Get current user profile
- `PUT /api/user/profile` - Update profile
- `POST /api/user` - Create user (CompanyAdmin+)
- `PUT /api/user/{id}` - Update user (CompanyAdmin+)
- `DELETE /api/user/{id}` - Delete user (CompanyAdmin+)
- `PUT /api/user/{id}/activate` - Activate user
- `PUT /api/user/{id}/deactivate` - Deactivate user
- `PUT /api/user/{id}/role` - Update user role
- `GET /api/user/{id}/tasks` - Get user's tasks
- `GET /api/user/{id}/permissions` - Get permissions
- `POST /api/user/{id}/permissions` - Update permissions
- `POST /api/user/profile/avatar` - Upload avatar

### Companies (`/api/company`)
- `GET /api/company` - Get all companies (SuperAdmin)
- `GET /api/company/{id}` - Get company by ID
- `GET /api/company/current` - Get current company
- `POST /api/company` - Create company (SuperAdmin)
- `PUT /api/company/{id}` - Update company
- `DELETE /api/company/{id}` - Delete company (SuperAdmin)
- `GET /api/company/{id}/statistics` - Get company statistics
- `PUT /api/company/{id}/subscription` - Update subscription (SuperAdmin)

### Clients (`/api/client`)
- `GET /api/client` - Get all clients
- `GET /api/client/{id}` - Get client by ID
- `POST /api/client` - Create client (Manager+)
- `PUT /api/client/{id}` - Update client (Manager+)
- `DELETE /api/client/{id}` - Delete client (Manager+)
- `GET /api/client/{id}/projects` - Get client projects
- `GET /api/client/{id}/tasks` - Get client tasks

### Projects (`/api/project`)
- `GET /api/project` - Get all projects
- `GET /api/project/{id}` - Get project by ID
- `POST /api/project` - Create project (Manager+)
- `PUT /api/project/{id}` - Update project (Manager+)
- `DELETE /api/project/{id}` - Delete project (Manager+)
- `GET /api/project/{id}/tasks` - Get project tasks
- `POST /api/project/{id}/team-member` - Add team member
- `DELETE /api/project/{id}/team-member/{userId}` - Remove team member
- `GET /api/project/{id}/statistics` - Get project statistics

### Dashboard (`/api/dashboard`)
- `GET /api/dashboard` - Get dashboard data
- `GET /api/dashboard/company-overview` - Company overview (CompanyAdmin+)
- `GET /api/dashboard/system-overview` - System overview (SuperAdmin)
- `GET /api/dashboard/user-performance` - User performance metrics
- `GET /api/dashboard/recent-activities` - Recent activities

## Authentication & Authorization

### JWT Token Structure
```json
{
  "sub": "user-id",
  "email": "user@example.com",
  "name": "John Doe",
  "role": "Manager",
  "companyId": "company-id",
  "companyName": "ACME Corp",
  "exp": 1234567890,
  "iss": "TaskManagementSystem",
  "aud": "TaskManagementClients"
}
```

### User Roles & Permissions

1. **SuperAdmin**
   - Full system access
   - Manage all companies
   - System-wide statistics
   - No company restrictions

2. **CompanyAdmin**
   - Full company access
   - Manage users and roles
   - Company settings
   - View company statistics

3. **Manager**
   - Create/edit/delete tasks
   - Manage clients and projects
   - Assign tasks to users
   - View team performance

4. **User**
   - View assigned tasks
   - Update task status
   - Add comments/attachments
   - View own performance

5. **TaskAssigner**
   - Create and assign tasks
   - Limited management capabilities

## Multi-Tenant Architecture

### Data Isolation
- All entities include `CompanyId` field
- Automatic filtering based on user's company
- SuperAdmin bypass for cross-company access
- Database-level row security

### Tenant Resolution
- JWT claims-based tenant identification
- Middleware for automatic tenant context
- Query filters for data isolation
- Audit trails per company

## Key Features

### Task Management
- Comprehensive CRUD operations
- Task assignment and reassignment
- Status tracking with history
- Priority and category management
- Due date and reminder system
- Subtask support
- File attachments
- Comment threads
- Recurring tasks
- Task templates

### Project Management
- Project creation and tracking
- Team member management
- Client association
- Budget tracking
- Timeline management
- Milestone tracking

### Reporting & Analytics
- Task statistics
- User performance metrics
- Project progress tracking
- Company-wide analytics
- Custom report generation

### Real-time Features
- SignalR for live notifications
- Task assignment notifications
- Status update alerts
- Comment notifications
- System announcements

## Security Features

### Authentication
- JWT with refresh tokens
- Password hashing (BCrypt)
- Email verification
- Password reset flow
- Account lockout protection
- Two-factor authentication (ready)

### Authorization
- Role-based access control
- Resource-based permissions
- API key authentication
- CORS configuration
- Rate limiting

### Data Protection
- SQL injection prevention
- XSS protection
- CSRF tokens
- Input validation
- Audit logging

## Performance Optimizations

### Database
- Indexed foreign keys
- Composite indexes for searches
- Filtered indexes for soft deletes
- Query optimization
- Connection pooling

### Caching
- Memory caching for frequent data
- Distributed cache ready
- Cache invalidation strategies
- Response caching

### API
- Pagination for all lists
- Async/await throughout
- Minimal DTOs
- Lazy loading prevention
- Efficient queries

## Error Handling

### HTTP Status Codes
- `200 OK` - Successful request
- `201 Created` - Resource created
- `204 No Content` - Successful delete
- `400 Bad Request` - Invalid input
- `401 Unauthorized` - Not authenticated
- `403 Forbidden` - Not authorized
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

### Error Response Format
```json
{
  "error": {
    "message": "Error description",
    "type": "ExceptionType",
    "stackTrace": "...",
    "innerException": "..."
  },
  "timestamp": "2024-01-15T10:30:00Z",
  "path": "/api/task/123"
}
```

## Development Setup

### Prerequisites
- .NET 8 SDK
- Docker Desktop
- SQL Server Management Studio (optional)
- Visual Studio 2022 or VS Code

### Quick Start
```bash
# Clone repository
git clone [repository-url]

# Start database
cd Database/Docker
docker-compose up -d

# Run migrations
cd ../../Backend
dotnet ef database update -p TaskManagement.Infrastructure -s TaskManagement.API

# Run API
cd TaskManagement.API
dotnet run
```

### Environment Variables
- `CONNECTION_STRING` - Database connection
- `JWT_SECRET` - JWT signing key
- `JWT_ISSUER` - Token issuer
- `JWT_AUDIENCE` - Token audience
- `SMTP_HOST` - Email server
- `SMTP_PORT` - Email port
- `SMTP_USERNAME` - Email username
- `SMTP_PASSWORD` - Email password

## Testing

### Unit Tests
- Service layer tests
- Repository tests
- Validation tests
- Mapping tests

### Integration Tests
- API endpoint tests
- Database tests
- Authentication flow tests
- Authorization tests

### Postman Collection
- Complete API coverage
- Environment variables
- Test scripts
- Documentation

## Deployment

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY . .
EXPOSE 80
ENTRYPOINT ["dotnet", "TaskManagement.API.dll"]
```

### Health Checks
- Database connectivity
- Service availability
- Memory usage
- Response time

### Monitoring
- Serilog integration
- Application Insights ready
- Performance counters
- Custom metrics

## Version History

### v1.0.0 (Current)
- Initial release
- Core task management
- Multi-tenant support
- JWT authentication
- Basic reporting

### Roadmap
- Mobile API endpoints
- Advanced reporting
- AI task suggestions
- Third-party integrations
- Workflow automation

## Support

### Known Issues
- Update operations need AutoMapper refinement
- Some error messages need improvement
- Notification service partially implemented

### Troubleshooting
- Check database connection
- Verify JWT configuration
- Review CORS settings
- Check file permissions

### Contact
- API Documentation: `/swagger`
- GitHub Issues: [repository-issues]
- Email: support@taskmanagement.com
