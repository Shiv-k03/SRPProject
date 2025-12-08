using FluentValidation;
using SRP.Model.DTOs.Requests;
using SRP.Repository.Enums;

namespace SRP.Business.Validators
{
    public class TeacherRequestValidator : AbstractValidator<TeacherRequest>
    {
        public TeacherRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(250).WithMessage("Email cannot exceed 100 characters");

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

            RuleFor(x => x.Role)
                .Must(r => r == UserRole.HOD || r == UserRole.Teacher)
                .WithMessage("Role must be either HOD or Teacher");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Department ID is required");

            RuleFor(x => x.EmployeeCode)
                .NotEmpty().WithMessage("Employee code is required")
                .MaximumLength(20).WithMessage("Employee code cannot exceed 20 characters");

            RuleFor(x => x.Qualification)
                .NotEmpty().WithMessage("Qualification is required")
                .MaximumLength(100).WithMessage("Qualification cannot exceed 100 characters");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15).WithMessage("Phone number cannot exceed 15 characters")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.JoiningDate)
                .NotEmpty().WithMessage("Joining date is required")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Joining date cannot be in the future");
        }
    }
}