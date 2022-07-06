using FluentValidation;

namespace InterviewProject.Controllers.Models.Requests.Validators
{
    public class ApiAddTagRequestValidator : AbstractValidator<ApiAddTagRequest>
    {
        public ApiAddTagRequestValidator()
        {
            this.RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
