using System.Threading.Tasks;

namespace TaskManagement.API.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendEmailVerificationAsync(string email, string verificationToken);
        Task SendPasswordResetEmailAsync(string email, string resetToken);
        Task SendTaskAssignmentEmailAsync(string email, string assigneeName, string taskTitle, string taskDescription);
        Task SendTaskStatusUpdateEmailAsync(string email, string taskTitle, string oldStatus, string newStatus);
        Task SendTaskReminderEmailAsync(string email, string taskTitle, DateTime dueDate);
        Task SendWelcomeEmailAsync(string email, string userName, string companyName);
    }
}
