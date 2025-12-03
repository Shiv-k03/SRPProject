namespace SRP.Business.Validators
{
    public class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
    {
        public CreateDepartmentRequestValidator()
        {
            RuleFor(x => x.DepartmentName)
                .NotEmpty().WithMessage("Department name is required")
                .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters");

            RuleFor(x => x.DepartmentCode)
                .NotEmpty().WithMessage("Department code is required")
                .MaximumLength(10).WithMessage("Department code cannot exceed 10 characters");

            RuleFor(x => x.DepartmentType)
                .IsInEnum().WithMessage("Invalid department type");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
