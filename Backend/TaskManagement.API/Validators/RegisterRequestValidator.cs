using FluentValidation;
using TaskManagement.API.Models.DTOs.Request;

namespace TaskManagement.API.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("First name can only contain letters and spaces");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Last name can only contain letters and spaces");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required")
                .Must(BeAValidRole).WithMessage("Invalid role specified");

            RuleFor(x => x.CompanyId)
                .NotEmpty().When(x => x.Role != "SuperAdmin")
                .WithMessage("Company ID is required for non-SuperAdmin users");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Invalid phone number format");

            RuleFor(x => x.Department)
                .MaximumLength(50).WithMessage("Department must not exceed 50 characters");

            RuleFor(x => x.JobTitle)
                .MaximumLength(100).WithMessage("Job title must not exceed 100 characters");
        }

        private bool BeAValidRole(string role)
        {
            var validRoles = new[] { "SuperAdmin", "CompanyAdmin", "Manager", "User", "TaskAssigner" };
            return validRoles.Contains(role);
        }
    }
}
