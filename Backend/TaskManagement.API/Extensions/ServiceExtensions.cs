using TaskManagement.API.Services.Interfaces;
using TaskManagement.API.Services.Implementation;
using TaskManagement.API.Services.Background;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using TaskManagement.API.Validators;

namespace TaskManagement.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureApplicationServices(this IServiceCollection services)
        {
            // Add Repository services
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add Business services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IDashboardService, DashboardService>();

            // Add Helper services
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add Background Services
            services.AddHostedService<EmailNotificationService>();

            // Add FluentValidation
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        }
    }
}