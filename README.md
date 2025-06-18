# Task Management System

A comprehensive multi-tenant task management system built with ASP.NET Core 8, Entity Framework Core, and SQL Server.

## 🚀 Features

- **Multi-tenant Architecture**: Complete data isolation between companies
- **Role-based Access Control**: SuperAdmin, CompanyAdmin, Manager, User, TaskAssigner
- **JWT Authentication**: Secure token-based authentication with refresh tokens
- **RESTful API**: Clean API design with Swagger documentation
- **SQL Server Database**: Optimized schema with 15+ tables
- **Clean Architecture**: Following SOLID principles and clean code practices
- **Real-time Updates**: SignalR integration for live notifications

## 📋 Prerequisites

- .NET 8.0 SDK
- SQL Server 2022 (or Docker Desktop)
- Visual Studio 2022 or VS Code
- Git

## 🛠️ Quick Start

### Windows Setup Guide
For detailed setup instructions, please refer to:
- [SETUP-GUIDE-COMPLETE.md](SETUP-GUIDE-COMPLETE.md) - Comprehensive setup guide
- [README-WINDOWS-SETUP.md](README-WINDOWS-SETUP.md) - Windows-specific instructions

### Quick Setup Steps:
1. Clone the repository
2. Choose database setup method:
   - **Option A**: Run `setup-database-windows.bat` (uses Docker)
   - **Option B**: Run `setup-database-local-windows.bat` (uses local SQL Server)
3. Update connection string in `Backend/TaskManagement.API/appsettings.json`
4. Run the application:
   ```bash
   cd Backend
   dotnet restore
   dotnet build
   cd TaskManagement.API
   dotnet run
   ```

The API will be available at:
- API: https://localhost:7237 or http://localhost:5237
- Swagger UI: https://localhost:7237/swagger

## 📁 Project Structure

```
TaskManagementSystem/
├── Backend/
│   ├── TaskManagement.API/          # Web API project
│   │   ├── Controllers/             # API endpoints
│   │   ├── Models/                  # DTOs and ViewModels
│   │   ├── Services/                # Business logic
│   │   ├── Middlewares/             # Custom middleware
│   │   ├── Validators/              # Input validation
│   │   └── Extensions/              # Extension methods
│   ├── TaskManagement.Core/         # Domain entities and interfaces
│   └── TaskManagement.Infrastructure/ # Data access and external services
├── Database/
│   ├── Docker/                      # Docker configuration
│   └── Scripts/                     # SQL initialization scripts
├── Documentation/                   # Project documentation
└── Tests/                          # Unit and integration tests
```

## 🗄️ Database Schema

The system includes the following main entities:
- **Companies**: Multi-tenant foundation
- **Users**: Authentication and authorization
- **Tasks**: Core business entity with full CRUD
- **SubTasks**: Task breakdown structure
- **Projects**: Project management
- **Clients**: Client relationship management
- **ChatGroups & Messages**: Real-time communication
- **Notifications**: User notifications
- **TaskAttachments**: File attachments
- **ActivityLogs**: Audit trails
- **UserPermissions**: Fine-grained permissions

## 🔐 Authentication

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "superadmin@taskmanagement.com",
  "password": "Admin@123"
}
```

### Default Test Users
After running the seed scripts, you can login with:

| Role | Email | Password |
|------|-------|----------|
| SuperAdmin | superadmin@taskmanagement.com | SuperAdmin123! |
| Company Admin | admin@techcorp.com | Admin123! |
| Manager | manager@techcorp.com | Manager123! |
| User | user@techcorp.com | User123! |

## 🔑 API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - Register new user (Admin only)
- `POST /api/auth/refresh-token` - Refresh access token
- `POST /api/auth/logout` - Logout user
- `POST /api/auth/change-password` - Change password
- `POST /api/auth/forgot-password` - Request password reset
- `POST /api/auth/reset-password` - Reset password
- `GET /api/auth/verify-email/{token}` - Verify email

### Users
- `GET /api/user` - Get all users
- `GET /api/user/{id}` - Get user by ID
- `POST /api/user` - Create new user
- `PUT /api/user/{id}` - Update user
- `DELETE /api/user/{id}` - Delete user

### Tasks
- `GET /api/task` - Get all tasks
- `GET /api/task/{id}` - Get task by ID
- `POST /api/task` - Create new task
- `PUT /api/task/{id}` - Update task
- `DELETE /api/task/{id}` - Delete task
- `POST /api/task/{id}/assign` - Assign task to user
- `PUT /api/task/{id}/status` - Update task status

### Companies
- `GET /api/company` - Get all companies (SuperAdmin only)
- `GET /api/company/{id}` - Get company by ID
- `POST /api/company` - Create new company
- `PUT /api/company/{id}` - Update company
- `DELETE /api/company/{id}` - Delete company

### Projects
- `GET /api/project` - Get all projects
- `GET /api/project/{id}` - Get project by ID
- `POST /api/project` - Create new project
- `PUT /api/project/{id}` - Update project
- `DELETE /api/project/{id}` - Delete project

### Clients
- `GET /api/client` - Get all clients
- `GET /api/client/{id}` - Get client by ID
- `POST /api/client` - Create new client
- `PUT /api/client/{id}` - Update client
- `DELETE /api/client/{id}` - Delete client

## 🛡️ Security Features

- JWT Bearer token authentication
- Password hashing with BCrypt
- Role-based authorization
- Multi-tenant data isolation
- CORS configuration
- SQL injection prevention
- XSS protection
- Rate limiting
- Input validation with FluentValidation

## 🔧 Configuration

Update `appsettings.json` for your environment:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskManagementDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-at-least-32-characters-long",
    "Issuer": "TaskManagementSystem",
    "Audience": "TaskManagementClients",
    "ExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  }
}
```

For Windows Authentication:
```json
"DefaultConnection": "Server=localhost;Database=TaskManagementDB;Trusted_Connection=True;TrustServerCertificate=true;"
```

## 📊 Database Management

### Connect with SQL Server Management Studio
- Server: localhost (or your server name)
- Authentication: SQL Server Authentication or Windows Authentication
- Database: TaskManagementDB

### Entity Framework Migrations
```bash
# Add a new migration
dotnet ef migrations add YourMigrationName -p TaskManagement.Infrastructure -s TaskManagement.API

# Update database
dotnet ef database update -p TaskManagement.Infrastructure -s TaskManagement.API
```

## 🧪 Testing

### Run Unit Tests
```bash
cd Backend
dotnet test
```

### Postman Collection
Import the Postman collection from `Tests/Postman/TaskManagement-Complete-Collection.postman_collection.json`

### Test Backend Endpoints
```bash
cd Tests
./backend-test-suite.sh
```

## 🚀 Deployment

### Docker Deployment
```bash
# Build Docker image
docker build -t taskmanagement-api .

# Run container
docker run -d -p 5194:80 --name taskmanagement-api taskmanagement-api
```

### Production Checklist
- [ ] Update connection strings
- [ ] Configure JWT secrets
- [ ] Set up SSL certificates
- [ ] Configure CORS policies
- [ ] Set up monitoring and logging
- [ ] Configure backup strategies
- [ ] Set up CI/CD pipeline

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License.

## 🆘 Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Ensure Docker is running
   - Check if SQL Server container is healthy
   - Verify connection string in appsettings.json

2. **Migration Errors**
   - Delete existing migrations in Infrastructure/Migrations
   - Drop and recreate database
   - Run migrations again

3. **Authentication Issues**
   - Check JWT secret key length (min 32 characters)
   - Verify token expiry settings
   - Ensure clock synchronization

## 📞 Support

For questions or support, please open an issue in the repository.

---

**Author**: Rajat Rajawat

**Note**: This is a comprehensive backend API for the Task Management System built for educational purposes.
