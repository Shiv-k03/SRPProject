namespace SRP.Business.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username)
               .NotEmpty().WithMessage("Username is required")
               .MinimumLength(3).WithMessage("Username must be at least 3 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required"); 
        }
    }
}