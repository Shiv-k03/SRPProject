using FluentValidation;
using SRP.Model.DTOs.Requests;

namespace SRP.Business.Validators
{
    public class SubjectRequestValidator : AbstractValidator<SubjectRequest>
    {
        public SubjectRequestValidator()
        {
            RuleFor(x => x.SubjectName)
                .NotEmpty().WithMessage("Subject name is required")
                .MaximumLength(100).WithMessage("Subject name cannot exceed 100 characters");

            RuleFor(x => x.SubjectCode)
                .NotEmpty().WithMessage("Subject code is required")
                .MaximumLength(20).WithMessage("Subject code cannot exceed 20 characters");

            RuleFor(x => x.Credits)
                .GreaterThan(0).WithMessage("Credits must be greater than 0")
                .LessThanOrEqualTo(10).WithMessage("Credits cannot exceed 10");

            RuleFor(x => x.Semester)
                .GreaterThan(0).WithMessage("Semester must be greater than 0")
                .LessThanOrEqualTo(8).WithMessage("Semester cannot exceed 8");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Department ID is required");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.MaxMarks)
                .GreaterThan(0).WithMessage("Max marks must be greater than 0");

            RuleFor(x => x.PassingMarks)
                .GreaterThan(0).WithMessage("Passing marks must be greater than 0")
                .LessThanOrEqualTo(x => x.MaxMarks).WithMessage("Passing marks cannot exceed max marks");
        }
    }
}

