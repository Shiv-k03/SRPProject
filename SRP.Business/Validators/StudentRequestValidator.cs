using FluentValidation;
using SRP.Model.DTOs.Requests;

namespace SRP.Business.Validators
{
    public class StudentRequestValidator : AbstractValidator<StudentRequest>
    {
        public StudentRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.Password)
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(50).WithMessage("Password cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.Password));

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Department ID is required");

            RuleFor(x => x.RollNumber)
                .NotEmpty().WithMessage("Roll number is required")
                .MaximumLength(20).WithMessage("Roll number cannot exceed 20 characters");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateTime.UtcNow.AddYears(-15)).WithMessage("Student must be at least 15 years old");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15).WithMessage("Phone number cannot exceed 15 characters")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Address));

            RuleFor(x => x.AdmissionDate)
                .NotEmpty().WithMessage("Admission date is required")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Admission date cannot be in the future");

            RuleFor(x => x.CurrentSemester)
                .GreaterThan(0).WithMessage("Current semester must be greater than 0")
                .LessThanOrEqualTo(8).WithMessage("Current semester cannot exceed 8");
        }
    }
}