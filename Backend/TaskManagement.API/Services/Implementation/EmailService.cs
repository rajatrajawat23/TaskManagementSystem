using System.Net;
using System.Net.Mail;
using System.Text;
using TaskManagement.API.Services.Interfaces;

namespace TaskManagement.API.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly bool _enableSsl;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            // Read SMTP configuration
            _smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "";
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
            _fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@taskmanagement.com";
            _fromName = _configuration["EmailSettings:FromName"] ?? "Task Management System";
            _enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };
                
                mailMessage.To.Add(to);

                using var smtpClient = new SmtpClient(_smtpHost, _smtpPort)
                {
                    EnableSsl = _enableSsl,
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword)
                };

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}");
                throw;
            }
        }

        public async Task SendEmailVerificationAsync(string email, string verificationToken)
        {
            var verificationUrl = $"{_configuration["AppSettings:BaseUrl"]}/verify-email?token={verificationToken}";
            
            var subject = "Verify Your Email - Task Management System";
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
                        .content {{ background-color: #f4f4f4; padding: 20px; margin-top: 0; }}
                        .button {{ display: inline-block; padding: 10px 20px; background-color: #2196F3; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Email Verification</h1>
                        </div>
                        <div class='content'>
                            <p>Thank you for registering with Task Management System!</p>
                            <p>Please click the button below to verify your email address:</p>
                            <a href='{verificationUrl}' class='button'>Verify Email</a>
                            <p style='margin-top: 20px;'>Or copy and paste this link in your browser:</p>
                            <p style='word-break: break-all;'>{verificationUrl}</p>
                            <p style='margin-top: 30px;'>This link will expire in 24 hours.</p>
                        </div>
                        <div class='footer'>
                            <p>If you didn't create an account, you can safely ignore this email.</p>
                            <p>&copy; 2025 Task Management System. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            var resetUrl = $"{_configuration["AppSettings:BaseUrl"]}/reset-password?token={resetToken}";
            
            var subject = "Password Reset Request - Task Management System";
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #FF9800; color: white; padding: 20px; text-align: center; }}
                        .content {{ background-color: #f4f4f4; padding: 20px; margin-top: 0; }}
                        .button {{ display: inline-block; padding: 10px 20px; background-color: #FF9800; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Password Reset Request</h1>
                        </div>
                        <div class='content'>
                            <p>We received a request to reset your password.</p>
                            <p>Click the button below to reset your password:</p>
                            <a href='{resetUrl}' class='button'>Reset Password</a>
                            <p style='margin-top: 20px;'>Or copy and paste this link in your browser:</p>
                            <p style='word-break: break-all;'>{resetUrl}</p>
                            <p style='margin-top: 30px;'>This link will expire in 1 hour.</p>
                            <p>If you didn't request a password reset, please ignore this email.</p>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2025 Task Management System. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendTaskAssignmentEmailAsync(string email, string assigneeName, string taskTitle, string taskDescription)
        {
            var subject = $"New Task Assigned: {taskTitle}";
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
                        .content {{ background-color: #f4f4f4; padding: 20px; margin-top: 0; }}
                        .task-details {{ background-color: white; padding: 15px; border-radius: 5px; margin-top: 15px; }}
                        .button {{ display: inline-block; padding: 10px 20px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>New Task Assignment</h1>
                        </div>
                        <div class='content'>
                            <p>Hello {assigneeName},</p>
                            <p>A new task has been assigned to you:</p>
                            <div class='task-details'>
                                <h3>{taskTitle}</h3>
                                <p>{taskDescription}</p>
                            </div>
                            <a href='{_configuration["AppSettings:BaseUrl"]}/tasks' class='button'>View Task</a>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2025 Task Management System. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendTaskStatusUpdateEmailAsync(string email, string taskTitle, string oldStatus, string newStatus)
        {
            var subject = $"Task Status Updated: {taskTitle}";
            var statusColor = newStatus switch
            {
                "Completed" => "#4CAF50",
                "InProgress" => "#2196F3",
                "Review" => "#FF9800",
                "Cancelled" => "#F44336",
                _ => "#9E9E9E"
            };

            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: {statusColor}; color: white; padding: 20px; text-align: center; }}
                        .content {{ background-color: #f4f4f4; padding: 20px; margin-top: 0; }}
                        .status-change {{ background-color: white; padding: 15px; border-radius: 5px; margin-top: 15px; }}
                        .old-status {{ color: #666; text-decoration: line-through; }}
                        .new-status {{ color: {statusColor}; font-weight: bold; }}
                        .button {{ display: inline-block; padding: 10px 20px; background-color: {statusColor}; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Task Status Update</h1>
                        </div>
                        <div class='content'>
                            <p>The status of the following task has been updated:</p>
                            <div class='status-change'>
                                <h3>{taskTitle}</h3>
                                <p>Status changed from <span class='old-status'>{oldStatus}</span> to <span class='new-status'>{newStatus}</span></p>
                            </div>
                            <a href='{_configuration["AppSettings:BaseUrl"]}/tasks' class='button'>View Task</a>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2025 Task Management System. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendTaskReminderEmailAsync(string email, string taskTitle, DateTime dueDate)
        {
            var timeRemaining = dueDate - DateTime.UtcNow;
            var urgency = timeRemaining.TotalHours < 24 ? "urgent" : "upcoming";
            var headerColor = timeRemaining.TotalHours < 24 ? "#F44336" : "#FF9800";
            
            var subject = $"Task Reminder: {taskTitle} - Due {dueDate:MMM dd, yyyy}";
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: {headerColor}; color: white; padding: 20px; text-align: center; }}
                        .content {{ background-color: #f4f4f4; padding: 20px; margin-top: 0; }}
                        .task-info {{ background-color: white; padding: 15px; border-radius: 5px; margin-top: 15px; }}
                        .due-date {{ color: {headerColor}; font-weight: bold; font-size: 18px; }}
                        .button {{ display: inline-block; padding: 10px 20px; background-color: {headerColor}; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>{(urgency == "urgent" ? "Urgent Task Reminder" : "Task Reminder")}</h1>
                        </div>
                        <div class='content'>
                            <p>This is a reminder about your {urgency} task:</p>
                            <div class='task-info'>
                                <h3>{taskTitle}</h3>
                                <p class='due-date'>Due: {dueDate:dddd, MMMM dd, yyyy at h:mm tt}</p>
                                <p>Time remaining: {(int)timeRemaining.TotalHours} hours</p>
                            </div>
                            <a href='{_configuration["AppSettings:BaseUrl"]}/tasks' class='button'>View Task</a>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2025 Task Management System. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string email, string userName, string companyName)
        {
            var subject = "Welcome to Task Management System!";
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
                        .content {{ background-color: #f4f4f4; padding: 20px; margin-top: 0; }}
                        .features {{ background-color: white; padding: 15px; border-radius: 5px; margin-top: 15px; }}
                        .feature-item {{ margin: 10px 0; padding-left: 20px; }}
                        .button {{ display: inline-block; padding: 10px 20px; background-color: #2196F3; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Welcome to Task Management System!</h1>
                        </div>
                        <div class='content'>
                            <p>Hello {userName},</p>
                            <p>Welcome to {companyName}'s Task Management System! We're excited to have you on board.</p>
                            <div class='features'>
                                <h3>Here's what you can do:</h3>
                                <div class='feature-item'>✓ Create and manage tasks</div>
                                <div class='feature-item'>✓ Collaborate with your team</div>
                                <div class='feature-item'>✓ Track project progress</div>
                                <div class='feature-item'>✓ Set priorities and deadlines</div>
                                <div class='feature-item'>✓ Share files and comments</div>
                            </div>
                            <a href='{_configuration["AppSettings:BaseUrl"]}/dashboard' class='button'>Get Started</a>
                        </div>
                        <div class='footer'>
                            <p>Need help? Contact our support team at support@taskmanagement.com</p>
                            <p>&copy; 2025 Task Management System. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }
    }
}
