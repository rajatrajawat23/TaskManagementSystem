# 🚀 Task Management System - Enterprise SaaS Platform

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red.svg)](https://www.microsoft.com/en-us/sql-server/)
[![Docker](https://img.shields.io/badge/Docker-Ready-brightgreen.svg)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A comprehensive **multi-tenant SaaS task management platform** built with **ASP.NET Core 8**, **Entity Framework Core**, and **SQL Server**. Designed for enterprise-scale applications with complete data isolation, role-based access control, and advanced features including real-time notifications, background services, and comprehensive analytics.

## 🌟 Key Features

### 🏢 **Multi-Tenant Architecture**
- **Complete Data Isolation**: Each company operates in its own secure environment
- **Tenant Context Management**: Automatic tenant resolution and data filtering
- **Scalable Design**: Support for unlimited companies and users
- **Resource Allocation**: Per-company subscription limits and resource management

### 🔐 **Advanced Security & Authentication**
- **JWT Bearer Authentication**: Secure token-based authentication with refresh tokens
- **Role-Based Access Control**: 5-tier permission system (SuperAdmin → User)
- **Multi-Factor Ready**: Infrastructure prepared for 2FA implementation
- **Security Middleware**: XSS protection, SQL injection prevention, CORS management
- **Audit Trails**: Comprehensive logging and activity tracking

### 📋 **Comprehensive Task Management**
- **Full CRUD Operations**: Create, read, update, delete tasks with validation
- **Task Hierarchies**: Support for subtasks and task dependencies
- **Advanced Assignments**: Multi-user assignments with role-based permissions
- **Status Tracking**: Customizable workflow states with history
- **Priority Management**: Critical, High, Medium, Low priority levels
- **Recurring Tasks**: Automated task generation with flexible patterns
- **File Attachments**: Document management with secure file storage
- **Comment System**: Threaded discussions on tasks
- **Time Tracking**: Estimated vs actual hours with reporting

### 🎯 **Project & Client Management**
- **Project Lifecycle**: End-to-end project management with phases
- **Client Relationships**: CRM-style client management
- **Team Management**: Project-based team assignments
- **Budget Tracking**: Financial oversight and cost management
- **Milestone Management**: Project deliverables and timeline tracking
- **Resource Allocation**: Team capacity and workload management

### 📊 **Analytics & Reporting**
- **Real-time Dashboards**: Executive, manager, and user-level dashboards
- **Performance Metrics**: Individual and team productivity analytics
- **Advanced Reporting**: Automated report generation and distribution
- **Data Visualization**: Charts, graphs, and KPI tracking
- **Export Capabilities**: PDF, Excel, and CSV export options
- **Historical Analysis**: Trend analysis and forecasting

### 🔔 **Real-time Features**
- **SignalR Integration**: Live notifications and updates
- **Push Notifications**: Task assignments, status changes, deadlines
- **Activity Feeds**: Real-time activity streams
- **Collaboration Tools**: Live commenting and status updates

### ⚙️ **Background Services**
- **Recurring Task Engine**: Automated task creation based on schedules
- **Email Notification System**: Automated email alerts and reminders
- **Data Cleanup Service**: Automated data archiving and maintenance
- **Report Generation**: Scheduled report creation and distribution

## 🏗️ Architecture Overview

### **Clean Architecture Implementation**
```
┌─────────────────────────────────────────────────────────────┐
│                    API Layer (Presentation)                 │
│  Controllers • Middlewares • Validation • Auth Policies    │
└─────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────┐
│                   Application Layer                         │
│    Services • DTOs • Mappings • Business Logic            │
└─────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────┐
│                     Core Layer (Domain)                     │
│     Entities • Interfaces • Specifications • Rules        │
└─────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────┐
│                Infrastructure Layer (Data)                  │
│   EF Core • Repositories • External Services • Caching    │
└─────────────────────────────────────────────────────────────┘
```

### **Technology Stack**
- **Backend**: ASP.NET Core 8.0 Web API
- **Database**: SQL Server 2022 (Docker containerized)
- **ORM**: Entity Framework Core 8.0
- **Authentication**: JWT Bearer with refresh tokens
- **Real-time**: SignalR for live updates
- **Logging**: Serilog with structured logging
- **Documentation**: Swagger/OpenAPI 3.0
- **Testing**: Comprehensive Postman test suite (80+ tests)
- **Containerization**: Docker & Docker Compose
- **Architecture**: Clean Architecture with SOLID principles

### **Database Schema**
```
🏢 Companies (Multi-tenant foundation)
👥 Users (Role-based authentication)
📋 Tasks (Core business entity)
📝 SubTasks (Task breakdown structure)
🎯 Projects (Project management)
👤 Clients (Customer relationship management)
💬 ChatGroups & ChatMessages (Communication)
🔔 Notifications (Alert system)
📎 TaskAttachments (File management)
👥 UserPermissions (Fine-grained access control)
📊 ActivityLogs (Audit trails)
⚙️ SystemSettings (Configuration management)
📧 EmailTemplates (Communication templates)
🔄 RecurringTasks (Automation patterns)
```

## 🚀 Quick Start Guide

### **Prerequisites**
- **.NET 8.0 SDK** or later
- **Docker Desktop** (for SQL Server)
- **Git** for version control
- **IDE**: Visual Studio 2022, VS Code, or JetBrains Rider

### **1. Clone & Setup**
```bash
# Clone the repository
git clone https://github.com/your-username/TaskManagementSystem.git
cd TaskManagementSystem

# Start the database container
cd Database/Docker
docker-compose up -d

# Wait for SQL Server to be ready (30-60 seconds)
docker-compose logs -f sqlserver
```

### **2. Database Setup**
```bash
# Navigate to scripts directory
cd ../Scripts

# Run complete database migration (creates all tables, indexes, constraints)
sqlcmd -S localhost,1433 -U sa -P "SecureTask2025#@!" -i "00-CompleteMigration.sql"

# Run seed data (creates test companies, users, projects, tasks)
sqlcmd -S localhost,1433 -U sa -P "SecureTask2025#@!" -i "01-CompleteSeedData.sql"
```

### **3. Run the Application**
```bash
# Navigate to API project
cd ../../Backend/TaskManagement.API

# Restore dependencies and run
dotnet restore
dotnet run
```

### **4. Access the Application**
- **API Base URL**: `http://localhost:5175/api`
- **Swagger Documentation**: `http://localhost:5175`
- **Health Check**: `http://localhost:5175/health`

### **5. Test Login Credentials**
| Role | Email | Password | Company |
|------|-------|----------|---------|
| **SuperAdmin** | `superadmin@taskmanagement.com` | `Admin@123` | System-wide |
| **Company Admin** | `john.admin@techinnovations.com` | `Admin@123` | Tech Innovations |
| **Manager** | `sarah.manager@techinnovations.com` | `Manager@123` | Tech Innovations |
| **User** | `mike.user@techinnovations.com` | `User@123` | Tech Innovations |

## 📁 Project Structure

```
TaskManagementSystem/
├── 🗂️ Backend/
│   ├── 🌐 TaskManagement.API/           # Web API Layer
│   │   ├── 📡 Controllers/              # REST API endpoints (9 controllers)
│   │   ├── 🧩 Services/                 # Business logic services
│   │   │   ├── Implementation/          # Service implementations (13 services)
│   │   │   ├── Interfaces/              # Service contracts
│   │   │   └── Background/              # Background services (4 services)
│   │   ├── 🗂️ Models/                   # DTOs and ViewModels
│   │   │   ├── Entities/                # Domain entities
│   │   │   ├── DTOs/                    # Data transfer objects
│   │   │   └── ViewModels/              # Presentation models
│   │   ├── 🛡️ Middlewares/              # Custom middleware
│   │   ├── ✅ Validators/               # Input validation
│   │   ├── 🔧 Extensions/               # Extension methods
│   │   └── ⚙️ Program.cs                # Application startup
│   ├── 🏛️ TaskManagement.Core/          # Domain Layer
│   │   ├── 📋 Entities/                 # Domain entities (15+ entities)
│   │   ├── 🔌 Interfaces/               # Repository contracts
│   │   └── ⚡ Specifications/           # Query specifications
│   └── 🗄️ TaskManagement.Infrastructure/ # Data Access Layer
│       ├── 💾 Data/                     # EF Core context and configurations
│       ├── 📦 Repositories/             # Data access implementations
│       └── 🌐 External/                 # External service integrations
├── 🗃️ Database/
│   ├── 🐳 Docker/                       # Container configuration
│   │   ├── docker-compose.yml          # SQL Server setup
│   │   └── .env                         # Environment variables
│   ├── 📜 Scripts/                      # Database scripts
│   │   ├── 00-CompleteMigration.sql     # Complete DB setup
│   │   ├── 01-CompleteSeedData.sql      # Test data
│   │   └── Archive/                     # Legacy scripts
│   └── 📊 Workbench-Exports/           # MySQL Workbench exports
├── 📚 Documentation/
│   ├── 🌐 API/                          # API documentation
│   │   ├── Complete-API-Documentation.md
│   │   └── Background-Services-Documentation.md
│   ├── 🗄️ Database/                     # Database documentation
│   └── 📋 Analysis-Reports/             # Technical analysis
├── 🧪 Tests/
│   ├── 📮 Postman/                      # API test collections (80+ tests)
│   │   ├── TaskManagement-Complete-Collection.json
│   │   └── README.md                    # Testing guide
│   ├── 🔧 Unit/                         # Unit tests
│   └── 🔗 Integration/                  # Integration tests
└── 🗂️ CleanupArchive/                   # Historical files and scripts
```

## 🔌 API Endpoints

### **Authentication & Security** (`/api/auth`)
```http
POST   /api/auth/login              # User authentication
POST   /api/auth/register           # User registration (Admin+)
POST   /api/auth/refresh-token      # JWT token refresh
POST   /api/auth/logout             # User logout
POST   /api/auth/forgot-password    # Password reset request
POST   /api/auth/reset-password     # Password reset confirmation
GET    /api/auth/verify-email/{token} # Email verification
POST   /api/auth/change-password    # Password change
```

### **Task Management** (`/api/task`)
```http
GET    /api/task                    # Get tasks (filtering, pagination)
GET    /api/task/{id}               # Get task details
POST   /api/task                    # Create new task
PUT    /api/task/{id}               # Update task
DELETE /api/task/{id}               # Delete task (soft delete)
POST   /api/task/{id}/assign        # Assign task to user
PUT    /api/task/{id}/status        # Update task status
GET    /api/task/calendar/{year}/{month} # Calendar view
GET    /api/task/user/{userId}      # Get user's tasks
GET    /api/task/overdue            # Get overdue tasks
POST   /api/task/{id}/duplicate     # Duplicate task
GET    /api/task/statistics         # Task analytics
POST   /api/task/{id}/comment       # Add task comment
POST   /api/task/{id}/attachment    # Add file attachment
```

### **Project Management** (`/api/project`)
```http
GET    /api/project                 # Get all projects
GET    /api/project/{id}            # Get project details
POST   /api/project                 # Create project (Manager+)
PUT    /api/project/{id}            # Update project
DELETE /api/project/{id}            # Delete project
GET    /api/project/{id}/tasks      # Get project tasks
POST   /api/project/{id}/team-member # Add team member
DELETE /api/project/{id}/team-member/{userId} # Remove team member
GET    /api/project/{id}/statistics # Project analytics
```

### **User Management** (`/api/user`)
```http
GET    /api/user                    # Get all users (Manager+)
GET    /api/user/{id}               # Get user details
GET    /api/user/profile            # Get current user profile
PUT    /api/user/profile            # Update profile
POST   /api/user                    # Create user (CompanyAdmin+)
PUT    /api/user/{id}               # Update user (CompanyAdmin+)
DELETE /api/user/{id}               # Delete user (CompanyAdmin+)
PUT    /api/user/{id}/activate      # Activate user
PUT    /api/user/{id}/deactivate    # Deactivate user
PUT    /api/user/{id}/role          # Update user role
GET    /api/user/{id}/tasks         # Get user's tasks
POST   /api/user/profile/avatar     # Upload user avatar
```

### **Company Management** (`/api/company`)
```http
GET    /api/company                 # Get all companies (SuperAdmin)
GET    /api/company/{id}            # Get company details
GET    /api/company/current         # Get current company
POST   /api/company                 # Create company (SuperAdmin)
PUT    /api/company/{id}            # Update company
DELETE /api/company/{id}            # Delete company (SuperAdmin)
GET    /api/company/{id}/statistics # Company analytics
PUT    /api/company/{id}/subscription # Update subscription
```

### **Client Management** (`/api/client`)
```http
GET    /api/client                  # Get all clients
GET    /api/client/{id}             # Get client details
POST   /api/client                  # Create client (Manager+)
PUT    /api/client/{id}             # Update client
DELETE /api/client/{id}             # Delete client
GET    /api/client/{id}/projects    # Get client projects
GET    /api/client/{id}/tasks       # Get client tasks
```

### **Dashboard & Analytics** (`/api/dashboard`)
```http
GET    /api/dashboard               # Get dashboard data
GET    /api/dashboard/company-overview # Company metrics (Admin+)
GET    /api/dashboard/system-overview  # System metrics (SuperAdmin)
GET    /api/dashboard/user-performance # User performance data
GET    /api/dashboard/recent-activities # Recent activity feed
```

### **Notifications** (`/api/notification`)
```http
GET    /api/notification            # Get user notifications
POST   /api/notification            # Create notification
PUT    /api/notification/{id}/read  # Mark as read
DELETE /api/notification/{id}       # Delete notification
GET    /api/notification/unread-count # Get unread count
```

### **System Diagnostics** (`/api/diagnostics`)
```http
GET    /api/diagnostics/health      # System health check
GET    /api/diagnostics/database    # Database connectivity
GET    /api/diagnostics/performance # Performance metrics
GET    /api/diagnostics/logs        # System logs (SuperAdmin)
```

## 🛡️ Security & Authentication

### **Role-Based Access Control**

| Role | Permissions | Scope |
|------|------------|-------|
| **SuperAdmin** | Full system access, manage all companies | System-wide |
| **CompanyAdmin** | Full company access, manage users/settings | Company-wide |
| **Manager** | Create/manage tasks, projects, clients | Team/project level |
| **User** | View/update assigned tasks, own profile | Personal tasks |
| **TaskAssigner** | Create and assign tasks, limited management | Task creation |

### **JWT Token Structure**
```json
{
  "sub": "user-guid",
  "email": "user@company.com",
  "name": "John Doe",
  "role": "Manager",
  "companyId": "company-guid",
  "companyName": "Tech Innovations Inc",
  "exp": 1640995200,
  "iss": "TaskManagementSystem",
  "aud": "TaskManagementClients"
}
```

### **Multi-Tenant Data Isolation**
- **Automatic Filtering**: All queries automatically filter by CompanyId
- **Tenant Middleware**: Extracts tenant context from JWT claims
- **Database Security**: Row-level security and constraints
- **SuperAdmin Override**: System administrators can access all data

## 🧪 Testing & Quality Assurance

### **Comprehensive Test Suite**
- **80+ Postman Tests**: Complete API coverage with automated validation
- **Security Testing**: SQL injection, XSS, unauthorized access prevention
- **Performance Testing**: Response time validation, load testing
- **Integration Testing**: End-to-end workflow validation
- **Data Validation**: Input validation and edge case handling

### **Test Categories**
1. **Authentication & Authorization Tests**
2. **CRUD Operations for All Entities**
3. **Multi-tenant Data Isolation**
4. **Security Vulnerability Testing**
5. **Performance & Load Testing**
6. **Error Handling & Edge Cases**
7. **Background Service Testing**
8. **Real-time Feature Testing**

### **Running Tests**
```bash
# Install Newman (Postman CLI)
npm install -g newman

# Run complete test suite
cd Tests/Postman
newman run TaskManagement-Complete-Collection.postman_collection.json \
  --iteration-count 1 \
  --delay-request 100 \
  --timeout-request 10000
```

## ⚙️ Configuration & Deployment

### **Environment Configuration**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=TaskManagementDB;User Id=sa;Password=SecureTask2025#@!;TrustServerCertificate=true;"
  },
  "JwtSettings": {
    "SecretKey": "SuperSecureJWTKey2025TaskMgmt789ComplexTokenForAuthentication",
    "Issuer": "TaskManagementSystem",
    "Audience": "TaskManagementClients",
    "ExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "EnableSsl": true
  }
}
```

### **Docker Deployment**
```bash
# Build application image
docker build -t taskmanagement-api .

# Run with Docker Compose
docker-compose up -d

# Scale services
docker-compose up --scale api=3
```

### **Production Checklist**
- [ ] Update JWT secrets and database passwords
- [ ] Configure HTTPS/SSL certificates
- [ ] Set up reverse proxy (nginx/IIS)
- [ ] Configure CORS for production domains
- [ ] Set up monitoring and logging
- [ ] Configure backup strategies
- [ ] Set up CI/CD pipeline
- [ ] Configure email SMTP settings
- [ ] Set up health check monitoring
- [ ] Configure rate limiting
- [ ] Set up data retention policies

## 🔧 Advanced Features

### **Background Services**
1. **Recurring Task Service**: Automatically creates recurring tasks based on schedules
2. **Email Notification Service**: Sends automated emails for task assignments, deadlines
3. **Data Cleanup Service**: Archives old data and maintains database performance
4. **Report Generation Service**: Creates and distributes scheduled reports

### **Real-time Features**
- **SignalR Hubs**: Live notifications and updates
- **Push Notifications**: Browser and mobile notifications
- **Activity Streams**: Real-time activity feeds
- **Live Collaboration**: Real-time commenting and status updates

### **Performance Optimizations**
- **Database Indexing**: Optimized indexes for all search operations
- **Query Optimization**: Efficient EF Core queries with minimal N+1 problems
- **Caching Strategy**: Memory caching for frequently accessed data
- **Pagination**: Efficient pagination for all list operations
- **Async Operations**: Fully asynchronous API operations

## 📊 Analytics & Reporting

### **Dashboard Features**
- **Executive Dashboard**: High-level KPIs and company overview
- **Manager Dashboard**: Team performance and project status
- **User Dashboard**: Personal task tracking and productivity metrics
- **System Dashboard**: Technical metrics and system health (SuperAdmin)

### **Reporting Capabilities**
- **Task Analytics**: Completion rates, overdue tasks, productivity trends
- **User Performance**: Individual and team productivity metrics
- **Project Reports**: Timeline, budget, resource utilization
- **Company Insights**: Cross-departmental analytics and trends
- **Custom Reports**: Configurable reporting with export options

## 🛠️ Development & Contribution

### **Development Setup**
```bash
# Clone and setup
git clone [repository-url]
cd TaskManagementSystem

# Setup database
cd Database/Docker && docker-compose up -d

# Install dependencies
cd ../../Backend && dotnet restore

# Run migrations
cd TaskManagement.API
dotnet ef database update -p ../TaskManagement.Infrastructure

# Start development server
dotnet run --watch
```

### **Code Quality Standards**
- **Clean Architecture**: Separation of concerns and dependency inversion
- **SOLID Principles**: Object-oriented design best practices
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Loose coupling and testability
- **Async/Await**: Non-blocking operations throughout
- **Error Handling**: Comprehensive exception management
- **Logging**: Structured logging with Serilog
- **Validation**: Input validation with FluentValidation

### **Contributing Guidelines**
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Follow coding standards and write tests
4. Commit changes (`git commit -m 'Add amazing feature'`)
5. Push to branch (`git push origin feature/amazing-feature`)
6. Open a Pull Request

## 📚 Documentation

- **[Complete API Documentation](Documentation/API/Complete-API-Documentation.md)**: Comprehensive API guide
- **[Background Services Documentation](Documentation/API/Background-Services-Documentation.md)**: Background services guide
- **[Database Schema Documentation](Documentation/Database/)**: Database design and schemas
- **[Testing Guide](Tests/Postman/README.md)**: Complete testing documentation
- **[EF Core Migrations Guide](Documentation/Database/EF-Core-Migrations-Guide.md)**: Database migration guide

## 🆘 Troubleshooting

### **Common Issues**

**Database Connection Issues**
```bash
# Check if SQL Server container is running
docker ps | grep sqlserver

# Restart SQL Server container
docker-compose restart sqlserver

# Check database connectivity
dotnet ef database update --dry-run
```

**Authentication Issues**
```bash
# Verify JWT configuration
# Check appsettings.json JWT settings
# Ensure token hasn't expired
# Verify user exists and is active
```

**Performance Issues**
```bash
# Check database performance
# Review SQL execution plans
# Monitor memory usage
# Check for N+1 query problems
```

## 🔮 Roadmap & Future Enhancements

### **Version 2.0 (Planned)**
- [ ] **Mobile API**: Dedicated mobile endpoints and push notifications
- [ ] **Advanced Workflow**: Custom workflow designer and automation
- [ ] **AI Integration**: Task suggestions and intelligent scheduling
- [ ] **Advanced Reporting**: Interactive dashboards and custom reports
- [ ] **Third-party Integrations**: Slack, Microsoft Teams, Google Workspace
- [ ] **Advanced Security**: Two-factor authentication and SSO integration
- [ ] **Audit & Compliance**: Enhanced audit trails and compliance reporting
- [ ] **Multi-language Support**: Internationalization and localization

### **Version 3.0 (Future)**
- [ ] **Microservices Architecture**: Service decomposition for scale
- [ ] **Event Sourcing**: Event-driven architecture implementation
- [ ] **Machine Learning**: Predictive analytics and smart insights
- [ ] **Advanced Collaboration**: Real-time collaborative editing
- [ ] **Enterprise Features**: Advanced governance and enterprise controls

## 📞 Support & Contact

### **Technical Support**
- **API Documentation**: Available at `/swagger` when running locally
- **GitHub Issues**: [Create an issue](../../issues) for bug reports or feature requests
- **Email Support**: support@taskmanagement.com

### **Development Team**
- **Lead Developer**: Rajat Rajawat
- **Architecture**: Clean Architecture with SOLID principles
- **Technology Stack**: ASP.NET Core 8, EF Core, SQL Server, Docker

### **Community**
- **Contributing**: See [Contributing Guidelines](#development--contribution)
- **Code of Conduct**: Follow professional development standards
- **License**: MIT License - see [LICENSE](LICENSE) file for details

---

## 📈 Project Statistics

- **Lines of Code**: 15,000+ (C#, SQL, Documentation)
- **API Endpoints**: 50+ RESTful endpoints
- **Database Tables**: 15+ normalized tables with relationships
- **Test Coverage**: 80+ comprehensive API tests
- **Documentation**: 25+ markdown files with detailed guides
- **Background Services**: 4 automated services
- **Security Features**: 10+ security implementations
- **Performance Optimizations**: 20+ database indexes and query optimizations

---

**Built with ❤️ for enterprise task management**

*Last Updated: June 2025 | Version: 1.0.0 | Status: Production Ready*