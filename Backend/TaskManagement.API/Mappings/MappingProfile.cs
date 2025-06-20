using AutoMapper;
using TaskManagement.API.Models.DTOs.Request;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.Core.Entities;
using System.Text.Json;

namespace TaskManagement.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Task mappings
            CreateMap<Core.Entities.Task, TaskResponseDto>()
                .ForMember(dest => dest.AssignedToName, opt => opt.MapFrom(src => 
                    src.AssignedTo != null ? $"{src.AssignedTo.FirstName} {src.AssignedTo.LastName}" : null))
                .ForMember(dest => dest.AssignedByName, opt => opt.MapFrom(src => 
                    $"{src.AssignedBy.FirstName} {src.AssignedBy.LastName}"))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => 
                    src.Client != null ? src.Client.Name : null))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => 
                    src.Project != null ? src.Project.Name : null))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => 
                    string.IsNullOrEmpty(src.Tags) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(src.Tags, new JsonSerializerOptions())));

            CreateMap<CreateTaskRequestDto, Core.Entities.Task>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => 
                    src.Tags != null && src.Tags.Any() ? JsonSerializer.Serialize(src.Tags, new JsonSerializerOptions()) : null))
                .ForMember(dest => dest.RecurrencePattern, opt => opt.MapFrom(src => 
                    src.RecurrencePattern != null ? JsonSerializer.Serialize(src.RecurrencePattern, new JsonSerializerOptions()) : null));

            CreateMap<CreateTaskDto, Core.Entities.Task>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => 
                    src.Tags != null && src.Tags.Any() ? JsonSerializer.Serialize(src.Tags, new JsonSerializerOptions()) : "[]"))
                .ForMember(dest => dest.RecurrencePattern, opt => opt.MapFrom(src => 
                    !string.IsNullOrEmpty(src.RecurrencePattern) ? src.RecurrencePattern : ""))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => 
                    !string.IsNullOrEmpty(src.Priority) ? src.Priority : "Medium"))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 
                    !string.IsNullOrEmpty(src.Status) ? src.Status : "Pending"))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => 
                    !string.IsNullOrEmpty(src.Category) ? src.Category : ""))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => 
                    !string.IsNullOrEmpty(src.Description) ? src.Description : ""));

            CreateMap<UpdateTaskRequestDto, Core.Entities.Task>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => 
                    src.Tags != null && src.Tags.Any() ? JsonSerializer.Serialize(src.Tags, new JsonSerializerOptions()) : null))
                .ForMember(dest => dest.RecurrencePattern, opt => opt.MapFrom(src => 
                    src.RecurrencePattern != null ? JsonSerializer.Serialize(src.RecurrencePattern, new JsonSerializerOptions()) : null))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateTaskDto, Core.Entities.Task>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CompanyId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.TaskNumber, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedBy, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedTo, opt => opt.Ignore())
                .ForMember(dest => dest.Client, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.ParentTask, opt => opt.Ignore())
                .ForMember(dest => dest.SubTasks, opt => opt.Ignore())
                .ForMember(dest => dest.SubTaskItems, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.Attachments, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => 
                    src.Tags != null && src.Tags.Any() ? JsonSerializer.Serialize(src.Tags, new JsonSerializerOptions()) : null))
                .ForMember(dest => dest.RecurrencePattern, opt => opt.MapFrom(src => 
                    !string.IsNullOrEmpty(src.RecurrencePattern) ? src.RecurrencePattern : null));

            // SubTask mappings
            CreateMap<SubTask, SubTaskDto>()
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.SortOrder));

            // TaskComment mappings
            CreateMap<TaskComment, TaskCommentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => 
                    $"{src.User.FirstName} {src.User.LastName}"));

            // TaskAttachment mappings
            CreateMap<TaskAttachment, TaskAttachmentDto>()
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.FileType))
                .ForMember(dest => dest.UploadedByName, opt => opt.MapFrom(src => 
                    $"{src.UploadedBy.FirstName} {src.UploadedBy.LastName}"))
                .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
            
            CreateMap<User, UserResponseDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => 
                    src.Company != null ? src.Company.Name : null));

            CreateMap<RegisterRequestDto, User>();
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CompanyId, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiry, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetExpiry, opt => opt.Ignore())
                .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
                .ForMember(dest => dest.EmailVerified, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedTasks, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTasks, opt => opt.Ignore())
                .ForMember(dest => dest.ManagedProjects, opt => opt.Ignore())
                .ForMember(dest => dest.Notifications, opt => opt.Ignore())
                .ForMember(dest => dest.Permissions, opt => opt.Ignore());

            // Company mappings
            CreateMap<Company, CompanyResponseDto>()
                .ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => 
                    src.Users != null ? src.Users.Count(u => u.IsActive) : 0));
            
            CreateMap<CreateCompanyDto, Company>();
            CreateMap<UpdateCompanyDto, Company>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.Users, opt => opt.Ignore())
                .ForMember(dest => dest.Projects, opt => opt.Ignore())
                .ForMember(dest => dest.Clients, opt => opt.Ignore())
                .ForMember(dest => dest.Tasks, opt => opt.Ignore());

            // Project mappings
            CreateMap<Project, ProjectResponseDto>()
                .ForMember(dest => dest.ProjectManagerName, opt => opt.MapFrom(src => 
                    src.ProjectManager != null ? $"{src.ProjectManager.FirstName} {src.ProjectManager.LastName}" : null))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => 
                    src.Client != null ? src.Client.Name : null))
                .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => 
                    src.Tasks != null ? src.Tasks.Count : 0))
                .ForMember(dest => dest.TeamMembers, opt => opt.Ignore()) // Will be handled in service
                .ForMember(dest => dest.CompletedTaskCount, opt => opt.MapFrom(src => 
                    src.Tasks != null ? src.Tasks.Count(t => t.Status == "Completed") : 0))
                .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src =>
                    src.ProjectManager != null ? $"{src.ProjectManager.FirstName} {src.ProjectManager.LastName}" : null))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => new List<string>()));

            CreateMap<CreateProjectDto, Project>()
                .ForMember(dest => dest.TeamMembers, opt => opt.MapFrom(src => 
                    src.TeamMembers != null && src.TeamMembers.Any() ? JsonSerializer.Serialize(src.TeamMembers, new JsonSerializerOptions()) : null));

            CreateMap<UpdateProjectDto, Project>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CompanyId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.Client, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectManager, opt => opt.Ignore())
                .ForMember(dest => dest.Tasks, opt => opt.Ignore())
                .ForMember(dest => dest.TeamMembers, opt => opt.MapFrom(src => 
                    src.TeamMembers != null && src.TeamMembers.Any() ? JsonSerializer.Serialize(src.TeamMembers, new JsonSerializerOptions()) : null));

            // Client mappings
            CreateMap<Client, ClientResponseDto>()
                .ForMember(dest => dest.ProjectCount, opt => opt.MapFrom(src => 
                    src.Projects != null ? src.Projects.Count : 0))
                .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => 
                    src.Tasks != null ? src.Tasks.Count : 0));

            CreateMap<CreateClientDto, Client>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == "Active"));
            CreateMap<UpdateClientDto, Client>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CompanyId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.Projects, opt => opt.Ignore())
                .ForMember(dest => dest.Tasks, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == "Active"));

            // Notification mappings
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => 
                    $"{src.User.FirstName} {src.User.LastName}"));
        }
    }
}