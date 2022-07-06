using FluentValidation;

namespace InterviewProject.Controllers.Models.Requests.Validators
{
    public class ApiContractorUpdateRequestValidator : AbstractValidator<ApiContractorUpdateRequest>
    {
        public ApiContractorUpdateRequestValidator()
        {
            this.RuleFor(x => x.ContractDuration)
                .NotEmpty();

            this.RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
