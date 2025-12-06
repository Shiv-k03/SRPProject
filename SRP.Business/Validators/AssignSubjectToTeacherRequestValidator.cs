using FluentValidation;
using SRP.Model.DTOs.Requests;

namespace SRP.Business.Validators
{

    public class AssignSubjectToTeacherRequestValidator : AbstractValidator<AssignSubjectToTeacherRequest>
    {
        public AssignSubjectToTeacherRequestValidator()
        {
            RuleFor(x => x.TeacherId)
                .GreaterThan(0).WithMessage("Teacher ID is required");

            RuleFor(x => x.SubjectId)
                .GreaterThan(0).WithMessage("Subject ID is required");

            RuleFor(x => x.Semester)
                .GreaterThan(0).WithMessage("Semester must be greater than 0")
                .LessThanOrEqualTo(8).WithMessage("Semester cannot exceed 8");

            RuleFor(x => x.AcademicYear)
                .NotEmpty().WithMessage("Academic year is required")
                .MaximumLength(10).WithMessage("Academic year cannot exceed 10 characters");
        }
    }
}