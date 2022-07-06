using FluentValidation;

namespace InterviewProject.Controllers.Models.Requests.Validators
{
    public class ApiContractorRequestValidator : AbstractValidator<ApiContractorRequest>
    {
        public ApiContractorRequestValidator()
        {
            this.RuleFor(x => x.ContractDuration)
                .NotEmpty();

            this.RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
        }

    }
}
