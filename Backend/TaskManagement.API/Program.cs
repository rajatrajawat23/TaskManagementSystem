using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using TaskManagement.Infrastructure.Data;
using TaskManagement.API.Extensions;
using TaskManagement.API.Middlewares;
using TaskManagement.API.Hubs;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // Configure Swagger
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Task Management API",
            Version = "v1",
            Description = "A comprehensive task management system API"
        });

        // Add JWT Authentication
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });

    // Configure Database
    builder.Services.AddDbContext<TaskManagementDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Configure JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"];

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

    // Add Authorization
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("SuperAdmin", policy => policy.RequireRole("SuperAdmin"));
        options.AddPolicy("CompanyAdmin", policy => policy.RequireRole("SuperAdmin", "CompanyAdmin"));
        options.AddPolicy("Manager", policy => policy.RequireRole("SuperAdmin", "CompanyAdmin", "Manager"));
        options.AddPolicy("User", policy => policy.RequireRole("SuperAdmin", "CompanyAdmin", "Manager", "User", "TaskAssigner"));
    });

    // Configure AutoMapper
    builder.Services.AddAutoMapper(typeof(Program).Assembly);

    // Add custom services
    builder.Services.ConfigureApplicationServices();

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
    });

    // Add Health Checks
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<TaskManagementDbContext>();

    // Add SignalR
    builder.Services.AddSignalR();

    var app = builder.Build();

    // Test Database Connection
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();
            await dbContext.Database.CanConnectAsync();
            Log.Information("✅ Database is connected and running successfully");
        }
    }
    catch (Exception dbEx)
    {
        Log.Warning("⚠️ Database connection failed: {Error}", dbEx.Message);
        Log.Information("📝 Note: Application will continue to run, but database-dependent features may not work");
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API V1");
            c.RoutePrefix = string.Empty;
        });
    }

    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging();
    app.UseCors("AllowAll");
    app.UseStaticFiles(); // Serve static files from wwwroot

    // Add custom middleware
    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseMiddleware<TenantMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health");
    app.MapHub<NotificationHub>("/notificationHub");

    // Apply migrations on startup (commented out for now - we'll add migrations later)
    // using (var scope = app.Services.CreateScope())
    // {
    //     var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();
    //     dbContext.Database.Migrate();
    // }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}