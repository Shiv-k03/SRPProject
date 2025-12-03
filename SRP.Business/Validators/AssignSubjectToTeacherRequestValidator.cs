namespace SRP.Business.Validators
{
    public class AssignSubjectToTeacherRequestValidator : AbstractValidator<AssignSubjectToTeacherRequestDto>
    {
        public AssignSubjectToTeacherRequestValidator()
        {
            RuleFor(x => x.TeacherId)
                .GreaterThan(0).WithMessage("Teacher ID must be greater than 0");

            RuleFor(x => x.SubjectId)
                .GreaterThan(0).WithMessage("Subject ID must be greater than 0");

            RuleFor(x => x.Semester)
                .GreaterThan(0).WithMessage("Semester must be greater than 0")
                .LessThanOrEqualTo(10).WithMessage("Semester cannot exceed 10");

            RuleFor(x => x.AcademicYear)
                .NotEmpty().WithMessage("Academic year is required")
                .Matches(@"^\d{4}-\d{4}$").WithMessage("Academic year must be in format YYYY-YYYY (e.g., 2024-2025)");
        }
    }
}