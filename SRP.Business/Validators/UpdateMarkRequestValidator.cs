namespace SRP.Business.Validators
{
    public class UpdateMarkRequestValidator : AbstractValidator<UpdateMarkRequestDto>
    {
        public UpdateMarkRequestValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("Student ID is required");

            RuleFor(x => x.SubjectId)
                .GreaterThan(0).WithMessage("Subject ID is required");

            RuleFor(x => x.ObtainedMarks)
                .GreaterThanOrEqualTo(0).WithMessage("Obtained marks cannot be negative")
                .LessThanOrEqualTo(x => x.TotalMarks).WithMessage("Obtained marks cannot exceed total marks");

            RuleFor(x => x.TotalMarks)
                .GreaterThan(0).WithMessage("Total marks must be greater than 0");

            RuleFor(x => x.ExamType)
                .NotEmpty().WithMessage("Exam type is required")
                .MaximumLength(50).WithMessage("Exam type cannot exceed 50 characters");

            RuleFor(x => x.ExamDate)
                .NotEmpty().WithMessage("Exam date is required")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Exam date cannot be in the future");

            RuleFor(x => x.Remarks)
                .MaximumLength(200).WithMessage("Remarks cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }
}