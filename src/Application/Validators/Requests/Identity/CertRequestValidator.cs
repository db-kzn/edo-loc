
using EDO_FOMS.Application.Requests.Identity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EDO_FOMS.Application.Validators.Requests.Identity
{
    public class CertRequestValidator : AbstractValidator<CertCheckRequest>
    {
        public CertRequestValidator(IStringLocalizer<CertRequestValidator> localizer)
        {
            RuleFor(request => request.OrgInn);
            //.Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Organization INN is required"]);

            RuleFor(request => request.Snils)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["SNILS is required"]);

            RuleFor(request => request.Thumbprint)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Thumbprint is required"]);
        }
    }
}
