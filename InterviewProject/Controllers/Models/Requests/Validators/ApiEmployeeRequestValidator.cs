using FluentValidation;

namespace InterviewProject.Controllers.Models.Requests.Validators
{
    public class ApiEmployeeRequestValidator : AbstractValidator<ApiEmployeeRequest>
    {
        public ApiEmployeeRequestValidator()
        {
            this.RuleFor(x => x.RoleType)
                .NotEmpty();

            this.RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
