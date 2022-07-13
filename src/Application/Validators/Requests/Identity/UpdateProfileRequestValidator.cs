using EDO_FOMS.Application.Requests.Identity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EDO_FOMS.Application.Validators.Requests.Identity
{
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator(IStringLocalizer<UpdateProfileRequestValidator> localizer)
        {
            RuleFor(request => request.Title)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Title is required"]);

            RuleFor(request => request.Surname)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Surname is required"]);

            RuleFor(request => request.GivenName)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["GivenName is required"]);
        }
    }
}