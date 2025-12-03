namespace SRP.Business.Validators
{
    public class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequestDto>
    {
        public CreateStudentRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(250).WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(Constants.MinPasswordLength).WithMessage($"Password must be at least {Constants.MinPasswordLength} characters");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Department ID must be greater than 0");

            RuleFor(x => x.RollNumber)
                .NotEmpty().WithMessage("Roll number is required")
                .MaximumLength(20).WithMessage("Roll number cannot exceed 20 characters");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateTime.UtcNow.AddYears(-15)).WithMessage("Student must be at least 15 years old")
                .GreaterThan(DateTime.UtcNow.AddYears(-100)).WithMessage("Invalid date of birth");

            RuleFor(x => x.CurrentSemester)
                .GreaterThan(0).WithMessage("Current semester must be greater than 0")
                .LessThanOrEqualTo(10).WithMessage("Current semester cannot exceed 10");

            RuleFor(x => x.PhoneNumber)
                .MinimumLength(8).WithMessage("Phone number cannot be less then 8 characters")
                .MaximumLength(15).WithMessage("Phone number cannot exceed 15 characters")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Address));
        }
    }
}