using FluentValidation;
using TaskManagement.API.Models.DTOs.Request;

namespace TaskManagement.API.Validators
{
    public class CreateProjectValidator : AbstractValidator<CreateProjectDto>
    {
        public CreateProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required")
                .MaximumLength(200).WithMessage("Project name must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters");

            RuleFor(x => x.ManagerId)
                .NotEmpty().WithMessage("Project manager is required");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(x => new[] { "Planning", "Active", "OnHold", "Completed", "Cancelled" }.Contains(x))
                .WithMessage("Status must be Planning, Active, OnHold, Completed, or Cancelled");

            RuleFor(x => x.Budget)
                .GreaterThanOrEqualTo(0).When(x => x.Budget.HasValue)
                .WithMessage("Budget must be greater than or equal to 0");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required")
                .Must(BeAValidDate).WithMessage("Start date must be a valid date");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).When(x => x.EndDate.HasValue)
                .WithMessage("End date must be after start date");

            RuleForEach(x => x.TeamMemberIds).NotEmpty()
                .When(x => x.TeamMemberIds != null && x.TeamMemberIds.Any())
                .WithMessage("Team member ID cannot be empty");

            RuleForEach(x => x.Tags).NotEmpty()
                .When(x => x.Tags != null && x.Tags.Any())
                .WithMessage("Tag cannot be empty");
        }

        private bool BeAValidDate(DateTime date)
        {
            return date != default(DateTime);
        }
    }

    public class UpdateProjectValidator : AbstractValidator<UpdateProjectDto>
    {
        public UpdateProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required")
                .MaximumLength(200).WithMessage("Project name must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters");

            RuleFor(x => x.ManagerId)
                .NotEmpty().WithMessage("Project manager is required");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(x => new[] { "Planning", "Active", "OnHold", "Completed", "Cancelled" }.Contains(x))
                .WithMessage("Status must be Planning, Active, OnHold, Completed, or Cancelled");

            RuleFor(x => x.Budget)
                .GreaterThanOrEqualTo(0).When(x => x.Budget.HasValue)
                .WithMessage("Budget must be greater than or equal to 0");

            RuleFor(x => x.ActualCost)
                .GreaterThanOrEqualTo(0).When(x => x.ActualCost.HasValue)
                .WithMessage("Actual cost must be greater than or equal to 0");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).When(x => x.EndDate.HasValue)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.Progress)
                .InclusiveBetween(0, 100)
                .WithMessage("Progress must be between 0 and 100");
        }
    }
}