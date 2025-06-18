using FluentValidation;
using TaskManagement.API.Models.DTOs.Request;

namespace TaskManagement.API.Validators
{
    public class CreateCompanyValidator : AbstractValidator<CreateCompanyDto>
    {
        public CreateCompanyValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Company name is required")
                .MaximumLength(100).WithMessage("Company name must not exceed 100 characters");

            RuleFor(x => x.Domain)
                .NotEmpty().WithMessage("Domain is required")
                .MaximumLength(50).WithMessage("Domain must not exceed 50 characters")
                .Matches(@"^[a-zA-Z0-9][a-zA-Z0-9-]{0,48}[a-zA-Z0-9]$")
                .WithMessage("Domain must contain only letters, numbers, and hyphens, and cannot start or end with a hyphen");

            RuleFor(x => x.ContactEmail)
                .NotEmpty().WithMessage("Contact email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Contact email must not exceed 100 characters");

            RuleFor(x => x.ContactPhone)
                .Matches(@"^\+?[1-9]\d{1,14}$").When(x => !string.IsNullOrEmpty(x.ContactPhone))
                .WithMessage("Invalid phone number format");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Address must not exceed 500 characters");

            RuleFor(x => x.SubscriptionType)
                .NotEmpty().WithMessage("Subscription type is required")
                .Must(BeAValidSubscriptionType).WithMessage("Invalid subscription type. Must be Free, Premium, or Enterprise");

            RuleFor(x => x.MaxUsers)
                .GreaterThan(0).WithMessage("Maximum users must be greater than 0")
                .LessThanOrEqualTo(10000).WithMessage("Maximum users cannot exceed 10000");

            RuleFor(x => x.SubscriptionExpiryDate)
                .GreaterThan(DateTime.UtcNow).When(x => x.SubscriptionExpiryDate.HasValue)
                .WithMessage("Subscription expiry date must be in the future");
        }

        private bool BeAValidSubscriptionType(string subscriptionType)
        {
            var validTypes = new[] { "Free", "Premium", "Enterprise" };
            return validTypes.Contains(subscriptionType);
        }
    }
}
