using FluentValidation;

namespace Lab4.Api
{
    public class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequest>
    {
        public CreateStudentRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MinimumLength(3);

            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress();

            RuleFor(x => x.EnrollmentDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Enrollment date cannot be in the future");
        }
    }
}
