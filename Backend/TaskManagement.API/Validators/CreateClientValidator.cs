using FluentValidation;
using TaskManagement.API.Models.DTOs.Request;

namespace TaskManagement.API.Validators
{
    public class CreateClientValidator : AbstractValidator<CreateClientDto>
    {
        public CreateClientValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Client name is required")
                .MaximumLength(200).WithMessage("Client name must not exceed 200 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

            RuleFor(x => x.Phone)
                .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.Phone))
                .WithMessage("Phone number must not exceed 20 characters");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Address must not exceed 500 characters");

            RuleFor(x => x.City)
                .MaximumLength(100).WithMessage("City must not exceed 100 characters");

            RuleFor(x => x.State)
                .MaximumLength(100).WithMessage("State must not exceed 100 characters");

            RuleFor(x => x.Country)
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters");

            RuleFor(x => x.PostalCode)
                .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters");

            RuleFor(x => x.Website)
                .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.Website))
                .WithMessage("Invalid website URL format");

            RuleFor(x => x.Industry)
                .MaximumLength(100).WithMessage("Industry must not exceed 100 characters");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(x => new[] { "Active", "Inactive", "Prospect" }.Contains(x))
                .WithMessage("Status must be Active, Inactive, or Prospect");

            RuleForEach(x => x.Contacts).SetValidator(new ClientContactValidator());
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }

    public class ClientContactValidator : AbstractValidator<ClientContactDto>
    {
        public ClientContactValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Contact name is required")
                .MaximumLength(100).WithMessage("Contact name must not exceed 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Contact email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Phone)
                .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.Phone))
                .WithMessage("Phone number must not exceed 20 characters");

            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters");
        }
    }
}