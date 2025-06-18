using FluentValidation;
using TaskManagement.API.Models.DTOs.Request;

namespace TaskManagement.API.Validators
{
    public class CreateTaskValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Task title is required")
                .MaximumLength(200).WithMessage("Task title must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Priority is required")
                .Must(BeAValidPriority).WithMessage("Invalid priority. Must be Low, Medium, High, or Critical");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(BeAValidStatus).WithMessage("Invalid status. Must be Pending, InProgress, Review, Completed, or Cancelled");

            RuleFor(x => x.Category)
                .MaximumLength(50).WithMessage("Category must not exceed 50 characters");

            RuleFor(x => x.EstimatedHours)
                .GreaterThanOrEqualTo(0).When(x => x.EstimatedHours.HasValue)
                .WithMessage("Estimated hours must be greater than or equal to 0")
                .LessThanOrEqualTo(999.99m).When(x => x.EstimatedHours.HasValue)
                .WithMessage("Estimated hours must not exceed 999.99");

            // Remove strict due date validation - allow historical tasks
            // RuleFor(x => x.DueDate)
            //     .GreaterThan(DateTime.UtcNow.AddHours(-24)).When(x => x.DueDate.HasValue)
            //     .WithMessage("Due date must be in the future");

            RuleFor(x => x.StartDate)
                .LessThan(x => x.DueDate).When(x => x.StartDate.HasValue && x.DueDate.HasValue)
                .WithMessage("Start date must be before due date");

            RuleFor(x => x.AssignedToId)
                .NotEmpty().When(x => x.AssignedToId.HasValue)
                .WithMessage("Invalid assigned user ID");

            RuleFor(x => x.ClientId)
                .NotEmpty().When(x => x.ClientId.HasValue)
                .WithMessage("Invalid client ID");

            RuleFor(x => x.ProjectId)
                .NotEmpty().When(x => x.ProjectId.HasValue)
                .WithMessage("Invalid project ID");

            RuleForEach(x => x.Tags).NotEmpty()
                .When(x => x.Tags != null && x.Tags.Any())
                .WithMessage("Tag cannot be empty");

            RuleFor(x => x.RecurrencePattern)
                .Must(BeValidJsonObject).When(x => x.IsRecurring && !string.IsNullOrEmpty(x.RecurrencePattern))
                .WithMessage("Recurrence pattern must be a valid JSON object");

            RuleFor(x => x.RecurrencePattern)
                .NotEmpty().When(x => x.IsRecurring)
                .WithMessage("Recurrence pattern is required when task is recurring");
        }

        private bool BeAValidPriority(string priority)
        {
            var validPriorities = new[] { "Low", "Medium", "High", "Critical" };
            return validPriorities.Contains(priority);
        }

        private bool BeAValidStatus(string status)
        {
            var validStatuses = new[] { "Pending", "InProgress", "Review", "Completed", "Cancelled" };
            return validStatuses.Contains(status);
        }

        private bool BeValidJsonObject(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return true;

            try
            {
                var parsed = System.Text.Json.JsonSerializer.Deserialize<object>(json);
                return parsed != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
