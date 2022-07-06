using FluentValidation;

namespace InterviewProject.Controllers.Models.Requests.Validators
{
    public class ApiEmployeeUpdateRequestValidator : AbstractValidator<ApiEmployeeUpdateRequest>
    {
        public ApiEmployeeUpdateRequestValidator()
        {
            this.RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
