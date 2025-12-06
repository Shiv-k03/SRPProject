using FluentValidation;
using SRP.Model.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRP.Business.Validators
{
    public class AssignSubjectToStudentRequestValidator : AbstractValidator<AssignSubjectToStudentRequest>
    {
        public AssignSubjectToStudentRequestValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("Student ID is required");

            RuleFor(x => x.SubjectId)
                .GreaterThan(0).WithMessage("Subject ID is required");

            RuleFor(x => x.Semester)
                .GreaterThan(0).WithMessage("Semester must be greater than 0")
                .LessThanOrEqualTo(8).WithMessage("Semester cannot exceed 8");
        }
    }
}
